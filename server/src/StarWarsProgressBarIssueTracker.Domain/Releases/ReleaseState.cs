namespace StarWarsProgressBarIssueTracker.Domain.Releases;

public enum ReleaseState
{
    Unknown = 0,

    /// <summary>
    /// Release is opened but not yet planned with an future release date.
    /// </summary>
    Open = 1,

    /// <summary>
    /// Release is planned with a release date in the future.
    /// </summary>
    Planned = 2,

    /// <summary>
    /// Release is finished and released.
    /// </summary>
    Finished = 3,
}
