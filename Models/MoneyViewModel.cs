using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class MoneyViewModel
    {
        [Key]
        public int MoneyId { get; set; }

        /* User */

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /* Courses */

        [Required]
        public List<Courses> Courses { get; set; } = new List<Courses>();

        /* Money */

        [Required]
        [Display(Name = "Amount Due")]
        [DataType(DataType.Currency)]
        public decimal AmountDue { get; set; }
    }
}
