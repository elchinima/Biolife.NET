public class ForgotPasswordVm
{
    [Required, EmailAddress, MaxLength(120)]
    public string Email { get; set; } = null!;
}
