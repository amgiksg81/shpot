using System;
using System.Web.Script.Serialization;

namespace SHPOT_ViewModel
{
    public class UserPremiumVM
    {
        [ScriptIgnore]
        public int UserPremiumlID { get; set; }

        public string CustomerID { get; set; }

        public string TransactionID { get; set; }

        public string ClientToken { get; set; }

        public string PaymentMode { get; set; }

        public double Amount { get; set; }

        public bool IsActive { get; set; }

        public int UserID { get; set; }

        public int BusinessID { get; set; }

        public string PaymentNonce { get; set; }
    }
}
