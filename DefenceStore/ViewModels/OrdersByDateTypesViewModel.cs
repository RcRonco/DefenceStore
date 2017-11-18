using System;
using System.ComponentModel;

namespace DefenceStore.ViewModels
{
    public class OrdersByDateTypesViewModel
    {
        [DisplayName("Order Date")]
        public string OrderDate { get; set; }

        [DisplayName("Number Of Orders")]
        public int NumberOfOrders { get; set; }

    }
}