using System;
using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class BusinessRatingVM
    {
        [ScriptIgnore]
        public int BusinessRatingID { get; set; }

        [ScriptIgnore]
        public int BusinessID { get; set; }

        [ScriptIgnore]
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string ProfileImageUrl { get; set; }

        [ScriptIgnore]
        public string PlaceID { get; set; }

        public string Review { get; set; }

        public string Rating { get; set; }
    }
}