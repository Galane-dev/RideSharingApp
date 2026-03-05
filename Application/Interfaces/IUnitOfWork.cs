namespace RideSharing.Application.Interfaces
{
    /// <summary>
    /// Coordinates all repository operations and flushes them to storage in a single commit.
    /// Services interact with repositories through this interface rather than saving individually.
    /// </summary>
    public interface IUnitOfWork
    {
        IPassengerRepository Passengers { get; }

        IDriverRepository Drivers { get; }

        IRideRepository Rides { get; }

        /// <summary>
        /// Reloads all collections from disk and rewires repositories to the fresh data.
        /// Call before any read operation when multiple application instances may be running.
        /// </summary>
        void Reload();

        /// <summary>
        /// Persists all pending changes across every repository to the underlying storage.
        /// </summary>
        void Commit();
    }
}
