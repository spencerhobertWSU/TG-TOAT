using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TGTOAT.Data;

namespace TGTOAT.Models
{
    public class PaymentViewModel
    {
        [Key]
        public int MoneyId { get; set; }

        // First name
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // Last name
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // The courses the user is registered for
        [Required]
        public List<Courses> Courses { get; set; } = new List<Courses>();

        // The amount the user has paid for the courses
        [Required]
        [Display(Name = "Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }

        // The amount due for the courses
        [Required]
        [Display(Name = "Amount Due")]
        [DataType(DataType.Currency)]
        public decimal AmountDue { get; set; }

        // The Stripe publishable key
        [Required]
        public string PublishableKey { get; set; }

        // If the Stripe payment was successful
        public bool? didSucceed { get; set; }


        //notifications
        public List<Notifications> Notifications { get; set; }
    }
}
