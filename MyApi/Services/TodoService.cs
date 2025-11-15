namespace MyApi.Services;
public static class TodoService
{
    public static bool IsValidTitle(string? title)
        => !string.IsNullOrWhiteSpace(title);
}