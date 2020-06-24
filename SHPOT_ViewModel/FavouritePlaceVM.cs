using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class FavouritePlaceVM : BusinessVM
    {
        [ScriptIgnore]
        public int FavouritePlaceID { get; set; }

        [ScriptIgnore]
        public new int UserID { get; set; }

        public new int BusinessID { get; set; }

        [ScriptIgnore]
        public bool IsFavourite { get; set; }

        public new string PlaceID { get; set; }
    }
}
