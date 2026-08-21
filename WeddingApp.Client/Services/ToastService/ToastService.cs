namespace WeddingApp.Client.Services.ToastService;

public enum ToastType
{
    Success,
    Error,
    Info,
    Warning
}

public class ToastMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; }
    public int Duration { get; set; } = 3000;
}

public class ToastService
{
    public event Action<ToastMessage>? OnToast;

    public void Show(
        string message,
        ToastType type = ToastType.Info,
        int duration = 3000)
    {
        OnToast?.Invoke(new ToastMessage
        {
            Message = message,
            Type = type,
            Duration = duration
        });
    }

    public void Success(string message, int duration = 3000)
        => Show(message, ToastType.Success, duration);

    public void Error(string message, int duration = 4000)
        => Show(message, ToastType.Error, duration);

    public void Info(string message, int duration = 3000)
        => Show(message, ToastType.Info, duration);

    public void Warning(string message, int duration = 3500)
        => Show(message, ToastType.Warning, duration);
}