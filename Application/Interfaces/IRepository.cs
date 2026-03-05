namespace RideSharing.Application.Interfaces
{
    /// <summary>
    /// Generic repository contract providing standard CRUD operations.
    /// Concrete implementations handle the underlying storage mechanism.
    /// </summary>
    public interface IRepository<T>
    {
        /// <summary>Returns every entity of this type.</summary>
        List<T> GetAll();

        /// <summary>Returns the entity with the given ID, or null if not found.</summary>
        T? GetById(Guid id);

        /// <summary>Adds a new entity to the collection.</summary>
        void Add(T entity);

        /// <summary>Replaces an existing entity matched by ID.</summary>
        void Update(T entity);

        /// <summary>Removes the entity with the given ID.</summary>
        void Delete(Guid id);
    }
}
