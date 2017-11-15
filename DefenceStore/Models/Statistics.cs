using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DefenceStore.Models
{
    public class Statistics
    {
        [Key]
        public int ID;

        [Required]
        public string Data;
    }
}