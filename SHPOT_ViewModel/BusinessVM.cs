using System;
using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class BusinessVM
    {
        public int BusinessID { get; set; }

        public string BusinessName { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Location { get; set; }

        public string ContactNo { get; set; }

        public string Website { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Flavours { get; set; }

        public string PremiumImageContent { get; set; }

        public string Icon { get; set; }

        public string PlaceID { get; set; }

        public double AverageRating { get; set; } = 0;

        public int IsFavouritePlace { get; set; } = 0;

        public int UserID { get; set; }

        public string Distance { get; set; }

        public string Reviews { get; set; }

        public string Images { get; set; }
    }
}
