using HotChocolate.Pagination;
using HotChocolate.Types.Pagination;

namespace StarWarsProgressBarIssueTracker.App.Extensions;

public static class PageExtensions
{
    public static Connection<T> ToConnectionWithTotalCount<T>(this Page<T> page) where T : class
    {
        Connection<T> connection = new Connection<T>(
            page.Items.Select(t => new Edge<T>(t, page.CreateCursor)).ToArray(),
            new ConnectionPageInfo(
                page.HasNextPage,
                page.HasPreviousPage,
                CreateCursor(page.First, page.CreateCursor),
                CreateCursor(page.Last, page.CreateCursor)),
            page.TotalCount ?? 0);
        return connection;
    }

    private static string? CreateCursor<T>(T? item, Func<T, string> createCursor) where T : class
        => item is null ? null : createCursor(item);
}
