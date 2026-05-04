namespace Biolife.Application.ViewModels;

public class ErrorVm
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
}
