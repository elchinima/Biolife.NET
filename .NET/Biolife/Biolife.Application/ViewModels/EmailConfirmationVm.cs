public class EmailConfirmationVm
{
    public bool IsSuccess { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string ActionText { get; set; } = null!;
}
