public class TwoFactorLoginVm
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Enter the 6-digit code.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter the 6-digit code.")]
    public string Code { get; set; } = null!;

    public bool RememberMe { get; set; }

    public string Email { get; set; } = string.Empty;
}
