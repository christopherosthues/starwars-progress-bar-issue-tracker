namespace StarWarsProgressBarIssueTracker.Infrastructure.Gitlab.Networking.Extensions;

public static class IdExtensions
{
    public static string ToId(this string id)
    {
        return id.Split("/").Last();
    }
}
