using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefenceStore.Models
{
    public class OrderProduct
    {
		[Key][Required]
		public int ID { get; set; }

		[Required]
		[ForeignKey("Order")]
		public int OrderID { get; set; }

		[Required][ForeignKey("Product")]
		public int ProductID { get; set; }

		[Required]
		public uint Quantity { get; set; }
    }
}
