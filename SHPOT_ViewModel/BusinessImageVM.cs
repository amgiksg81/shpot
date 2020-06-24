using System;
using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class BusinessImageVM
    {
        public int BusinessImageID { get; set; }

        [ScriptIgnore]
        public int BusinessID { get; set; }

        public string ImageName { get; set; }

        [ScriptIgnore]
        public string ImageType { get; set; }

        public string ImagePath { get; set; }

        [ScriptIgnore]
        public bool IsPremiumImage { get; set; } = false;
    }
}