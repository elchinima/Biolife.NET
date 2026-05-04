public class RegisterVm
{
    [Required, MaxLength(25, ErrorMessage = "Max 25 characters.")]
    public string Name { get; set; } = null!;

    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
