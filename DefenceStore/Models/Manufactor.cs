using System;
using System.ComponentModel.DataAnnotations;

namespace DefenceStore.Models
{
    public class Manufactor
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        public string Desciption { get; set; }
        public int TotalProduct { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }
    }
}