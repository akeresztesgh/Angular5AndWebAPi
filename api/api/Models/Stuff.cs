using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class Stuff
    {

        [Key]
        public int Id { get; set; }

        public string String1 { get; set; }
        public string String2 { get; set; }
        public DateTime Date { get; set; }
    }
}