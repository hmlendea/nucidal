using System;

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
    public class EntityBase<TKey> : IEquatable<EntityBase<TKey>>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public TKey Id { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="EntityBase{TKey}"/> is equal to the current <see cref="EntityBase{TKey}"/>.
        /// </summary>
        /// <param name="other">The <see cref="EntityBase{TKey}"/> to compare with the current <see cref="EntityBase{TKey}"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="EntityBase{TKey}"/> is equal to the current <see cref="EntityBase{TKey}"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(EntityBase<TKey> other)
            => other is not null && Id.Equals(other.Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="EntityBase{TKey}"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="EntityBase{TKey}"/>.</param>
        /// <returns><c>true</c> if the specified object is equal to the current <see cref="EntityBase{TKey}"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
            => Equals(obj as EntityBase<TKey>);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
            => Id?.GetHashCode() ?? 0;
    }
}
