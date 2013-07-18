using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowerCatcher
{
    /// <summary>
    /// A view model for storing all game related information: score, tweet hashtag, etc.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        static MainViewModel instance;

        /// <summary>
        /// Instance for singleton access.
        /// </summary>
        public static MainViewModel Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        List<string> scores;

        /// <summary>
        /// List of high scores.
        /// </summary>
        public List<string> Scores
        {
            get { return scores; }
            set
            {
                scores = value;
                ChangeProperty("Scores");
            }
        }

        int followers;

        /// <summary>
        /// Number of items collected.
        /// </summary>
        public int Followers
        {
            get { return followers; }
            set
            {
                followers = value;
                ChangeProperty("Followers");
            }
        }

        double miles;

        /// <summary>
        /// Distance skated.
        /// </summary>
        public double Miles
        {
            get { return miles; }
            set
            {
                miles = value;
                ChangeProperty("Miles");
            }
        }

        private string hashtag;

        /// <summary>
        /// Hashtag used for searching Twitter.
        /// </summary>
        public string Hashtag
        {
            get { return this.hashtag; }
            set
            {
                if (this.hashtag != value)
                {
                    this.hashtag = value;
                    ChangeProperty("Hashtag");
                }
            }
        }

        string actualTweet;

        /// <summary>
        /// Tweet of the last follower collected.
        /// </summary>
        public string ActualTweet
        {
            get { return actualTweet; }
            set
            {
                actualTweet = value;
                ChangeProperty("ActualTweet");
            }
        }

        string actualUser;

        /// <summary>
        /// User name of the last follower collected.
        /// </summary>
        public string ActualUser
        {
            get { return actualUser; }
            set
            {
                actualUser = value;
                ChangeProperty("ActualUser");
            }
        }

        /// <summary>
        /// Initializes default values.
        /// </summary>
        public MainViewModel()
        {
            Instance = this;
            Scores = new List<string>();
            Scores.Add("1. Gnocci Urdangarin");
            Scores.Add("2. Rot Drigo");
            Scores.Add("3. Don Pito Corleone");
            Scores.Add("4. Bender Vending Rodriguez");
            Scores.Add("5. Sound, pls");
            Scores.Add("6. Culito Mussolini");
            Scores.Add("7. Estib Yos");
            Scores.Add("8. Tetitas Larue");
            Scores.Add("9. Pechitos Mc Tetis");
            Scores.Add("10. Mr Roboto");

            this.Hashtag = "hackw8";
            actualUser = "@you";
            ActualTweet = "¡Twittea con el hashtag #" + this.Hashtag + " para salir aquí!";
            Miles = 0;
            Followers = 0;
        }

        /// <summary>
        /// Used for MVVM value updating.
        /// </summary>
        /// <param name="name">Property name.</param>
        private void ChangeProperty(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}