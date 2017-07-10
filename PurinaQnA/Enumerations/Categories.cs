using PurinaQnA.Utils;

namespace PurinaQnA.Enumerations
{
    public class Categories : Enumeration
    {
        public Categories() { }
        public Categories(int value, string displayName) : base(value, displayName) { }
        public Categories(int value, string displayName, string feedUrl) : base(value, displayName) {
            _feedUrl = feedUrl;
        }

        private const string FeedBaseUrl = "http://tst2.purinamills.com";

        public static readonly Categories All = new Categories(0, "All");
        public static readonly Categories Horses = new Categories(1, "Horse", $"{FeedBaseUrl}/horse-feed");
        public static readonly Categories Cattle = new Categories(2, "Cattle", $"{FeedBaseUrl}/cattle-feed");
        public static readonly Categories BackyardPoultry = new Categories(3, "Backyard Poultry", $"{FeedBaseUrl}/chicken-feed");
        public static readonly Categories Dairy = new Categories(4, "Dairy", $"{FeedBaseUrl}/diary-feed");
        public static readonly Categories Goats = new Categories(5, "Goats", $"{FeedBaseUrl}/goat-feed");
        public static readonly Categories Swine = new Categories(6, "Swine", $"{FeedBaseUrl}/swine-feed");
        public static readonly Categories Rabbits = new Categories(7, "Rabbits", $"{FeedBaseUrl}/rabbit-food");
        public static readonly Categories SmallAnimals = new Categories(8, "Small Animals");
        public static readonly Categories Birds = new Categories(9, "Birds");
        public static readonly Categories Wildlife = new Categories(10, "Wildlife", $"{FeedBaseUrl}/game-feed");
        public static readonly Categories FishAquatics = new Categories(11, "Fish Aquatics");
        public static readonly Categories ShowAnimals = new Categories(12, "Show Animals", $"{FeedBaseUrl}/show-feed");
        public static readonly Categories Exotics = new Categories(13, "Exotics", $"{FeedBaseUrl}/exotic-animal-food");

        private readonly string _feedUrl;
        public string FeedUrl {
            get { return _feedUrl; }
        }
    }
}