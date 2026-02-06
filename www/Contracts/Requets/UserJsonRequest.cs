using System.ComponentModel.DataAnnotations;

namespace MyMicroservice.Contracts.Requests;

public record RegisterJsonRequest(
    [Required(ErrorMessage = "Login is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters")]
    string Login,

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    string Name,

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    string Password,

    [Required(ErrorMessage = "Role is required")]
    [StringLength(20, ErrorMessage = "Role must be at most 20 characters")]
    string Role 
);

public record LoginJsonRequest(
    [Required(ErrorMessage = "Login is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Login must be between 3 and 50 characters")]
    string Login,

    [Required(ErrorMessage = "Password is required")]
    string Password
);