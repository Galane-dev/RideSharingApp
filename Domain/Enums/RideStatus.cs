namespace RideSharing.Domain.Enums
{
    /// <summary>
    /// Represents the lifecycle status of a ride from request through completion.
    /// </summary>
    public enum RideStatus
    {
        Requested = 1,
        Accepted = 2,
        Completed = 3,
        Cancelled = 4
    }
}
