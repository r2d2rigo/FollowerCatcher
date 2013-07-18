using CommonDX;
using LinqToTwitter;
using SharpDX.Direct3D11;
using SharpDX.IO;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.Storage;

namespace FollowerCatcher
{
    /// <summary>
    /// Performs all Twitter related functionality (login, get user information, search...).
    /// </summary>
    public class TwitterClient
    {
        private WinRtAuthorizer authorizer;
        private TwitterContext context;
        private ImagingFactory2 factory;
        private Device1 device;

        /// <summary>
        /// Stores the user's avatars as textures drawable by the game.
        /// </summary>
        public List<Texture> AvatarTextures { get; private set; }

        /// <summary>
        /// Stores the last tweet of a specific user.
        /// </summary>
        public Dictionary<string, TweetInfo> Tweets { get; private set; }

        /// <summary>
        /// Creates the client adn initializes the members that will request a Twitter authorization and
        /// perform search tasks.
        /// </summary>
        /// <param name="device">DirectX device.</param>
        /// <param name="factory">Imaging factory.</param>
        public TwitterClient(Device1 device, ImagingFactory2 factory)
        {
            this.authorizer = new WinRtAuthorizer()
            {
                Credentials = new LocalDataCredentials()
                {
#error Please create an application in Twitter and set your own consumer key and secret.
                    ConsumerKey = null,
                    ConsumerSecret = null,
                },
                UseCompression = true,
                Callback = new Uri("http://followercatcher.codeplex.com/"),
            };

            this.device = device;
            this.factory = factory;
            this.AvatarTextures = new List<Texture>();
            this.Tweets = new Dictionary<string, TweetInfo>();

            LoadTexturesFromCache();
        }

        /// <summary>
        /// Searchs for the tweets containing a specific search string.
        /// </summary>
        /// <param name="tweetText">The search string.</param>
        public async void SearchTweets(string tweetText)
        {
            if (!this.authorizer.IsAuthorized)
            {
                await this.authorizer.AuthorizeAsync();
                this.context = new TwitterContext(authorizer);
            }

            string roamingFolder = ApplicationData.Current.RoamingFolder.Path + "\\";            

            var searchResult =
                (from search in context.Search
                    where search.Type == SearchType.Search &&
                    search.Query == tweetText
                    select search)
                .SingleOrDefault();

            foreach (var result in searchResult.Statuses)
            {
                bool alreadyExists = false;
                bool fileExists = true;
                HttpClient http = new HttpClient();
                HttpResponseMessage response = await http.GetAsync(result.User.ProfileImageUrl);
                string userAvatarName = result.User.Identifier.UserID + Path.GetExtension(result.User.ProfileImageUrl);

                Tweets[userAvatarName] = new TweetInfo("@" + result.User.Identifier.ScreenName, result.Text);

                try
                {
                    StorageFile existingFile = await ApplicationData.Current.RoamingFolder.GetFileAsync(userAvatarName);
                }
                catch (FileNotFoundException f)
                {
                    fileExists = false;
                }

                if (!fileExists)
                {
                    using (NativeFileStream file = new NativeFileStream(roamingFolder + userAvatarName, NativeFileMode.Create, NativeFileAccess.Write))
                    {
                        await response.Content.CopyToAsync(file.AsOutputStream().AsStreamForWrite());
                    }
                }

                for (int i = 0; i < AvatarTextures.Count; i++)
                {
                    if (AvatarTextures[i].TexturePath == userAvatarName)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    AddTextureFromFile(userAvatarName);
                }
            }
        }

        /// <summary>
        /// Checks for already saved avatars on the roaming folder and loads them.
        /// </summary>
        private async void LoadTexturesFromCache()
        {
            var fileList = await ApplicationData.Current.RoamingFolder.GetFilesAsync();

            for (int i = 0; i < fileList.Count; i++)
            {
                AddTextureFromFile(Path.GetFileName(fileList[i].Path));
            }
        }

        /// <summary>
        /// Opens an user's avatar previosly saved on disk and creates a DirectX texture with it.
        /// </summary>
        /// <param name="avatarName">Twitter user ID of the avatar's owner.</param>
        private async void AddTextureFromFile(string avatarName)
        {
            string roamingFolder = ApplicationData.Current.RoamingFolder.Path + "\\";

            BitmapSource bitmapSrc = TextureLoader.LoadBitmap(factory, roamingFolder + avatarName);
            var texture2D = TextureLoader.CreateTexture2DFromBitmap(device, bitmapSrc);

            lock (AvatarTextures)
            {
                AvatarTextures.Add(new Texture(avatarName, new ShaderResourceView(device, texture2D)));
            }
        }
    }
}
