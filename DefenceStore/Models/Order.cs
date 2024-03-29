﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefenceStore.Models
{
    public class Order
    {
		[Key][Required]
		public int ID { get; set; }

        [Required][ForeignKey("Customer")]
		public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [DataType(DataType.Currency)][Required]
		public string BillingType { get; set; }

        [Required]
		public string Address { get; set; }

        [DataType(DataType.DateTime)][Required]
		public DateTime Date { get; set; }

        public string Desciption { get; set; }

        [Required]
        public float TotalBill { get; set; }

        public List<OrderProduct> products { get; set; }
    }
}
