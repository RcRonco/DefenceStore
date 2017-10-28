using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefenceStore.Models
{
    public class Product
    {
        [Required][Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        public string Desciption { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public uint QuantityInStock { get; set; }

        [ForeignKey("Manufactor")]
        public int ManufactorId { get; set; }
    }
}
