using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MvcMovie.Models;

public class User
{
    public int Id { get; set; }

    [RegularExpression(@".+@.+", ErrorMessage = "Invalid email format")]
    [StringLength(30, MinimumLength = 3)]
    [Required]
    public string? Email { get; set; }

    [StringLength(30, MinimumLength = 6)]
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

    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    [Required]
    public DateTime BirthDate { get; set; }

    [Display(Name = "User Role")]
    [Required]
    public string? UserRole { get; set; }
}