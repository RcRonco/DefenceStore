using System.ComponentModel;

namespace DefenceStore.ViewModels
{
    public class OrdersBillingTypesViewModel
    {
        
        [DisplayName("Billing Type")]
        public string BillingType { get; set; }

        [DisplayName("Number Of Orders")]
        public int NumberOfOrders { get; set; }
    }
}