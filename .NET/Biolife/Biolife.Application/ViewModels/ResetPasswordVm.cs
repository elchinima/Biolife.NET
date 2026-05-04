public class ResetPasswordVm
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string Token { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
