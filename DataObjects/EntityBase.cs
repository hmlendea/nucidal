namespace NuciDAL.DataObjects
{
    /// <summary>
    /// Base class for entities with a string identifier.
    /// </summary>
    public class EntityBase : EntityBase<string> { }

    /// <summary>
    /// Base class for entities with a generic identifier.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier.</typeparam>
    public class EntityBase<TKey>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public TKey Id { get; set; }
    }
}
