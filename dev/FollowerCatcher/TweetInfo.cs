namespace FollowerCatcher
{
    /// <summary>
    /// Stores the user and text of a tweet.
    /// </summary>
    public class TweetInfo
    {
        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the tweet text.
        /// </summary>
        /// <value>
        /// The tweet text.
        /// </value>
        public string TweetText { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetInfo"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="tweetText">The tweet text.</param>
        public TweetInfo(string username, string tweetText)
        {
            this.Username = username;
            this.TweetText = tweetText;
        }
    }
}
