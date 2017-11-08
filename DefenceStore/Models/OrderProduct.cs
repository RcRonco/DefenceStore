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
        public virtual Order Order { get; set; }

        [Required][ForeignKey("Product")]
		public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [Required]
		public int Quantity { get; set; }
    }
}
