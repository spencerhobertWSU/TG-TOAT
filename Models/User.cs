using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TGTOAT.Data;

namespace TGTOAT.Models;

public class User
{
    public int Id { get; set; }

    [RegularExpression(@".+@.+", ErrorMessage = "Invalid email format")]
    [StringLength(30, MinimumLength = 3)]
    [Required]
    public string? Email { get; set; }

    [StringLength(100, MinimumLength = 6)]
    [Required]
    public string? Password { get; set; }

    [Display(Name = "First Name")]
    [StringLength(60, MinimumLength = 1)]
    [Required]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    [StringLength(60, MinimumLength = 1)]
    [Required]
    public string? LastName { get; set; }

    public string? FullName => $"{FirstName} {LastName}";

    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    [Required]
    public DateTime BirthDate { get; set; }

    [Display(Name = "User Role")]
    [Required]
    public string? UserRole { get; set; }

    [Display(Name = "ProfileImageBase64")]
    [Required]
    public string? ProfileImageBase64 { get; set; } = string.Empty;

    //Lazy Loading Nav Link
    public Address Address { get; set; }

    // Money information
    [Display(Name = "Amount Due")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18, 2)")]
    [DefaultValue(0)]
    public decimal AmountDue { get; set; }
}