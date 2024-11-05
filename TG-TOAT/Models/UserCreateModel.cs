using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Data;

namespace Models;

public class UserCreateModel
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email name is required.")]
    [MaxLength(100, ErrorMessage = "Max 100 characters allowed.")]
    [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(100, ErrorMessage = "Max 20 characters allowed.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]

    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Birthdate is required.")]
    [DataType(DataType.Date)]
    public DateOnly BirthDate { get; set; }

    [Display(Name = "User Role")]
    [Required]
    public string UserRole { get; set; }
}