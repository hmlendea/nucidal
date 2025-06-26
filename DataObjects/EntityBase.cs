using System;
using System.Reflection;
using NuciExtensions;

namespace NuciDAL.DataObjects
{
    /// <summary>
    /// Base class for entities with a string identifier.
    /// </summary>
    public class EntityBase : EntityBase<string>, IEquatable<EntityBase<string>> { }

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
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType().NotEquals(other.GetType()))
            {
                return false;
            }

            PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                object thisValue = prop.GetValue(this);
                object otherValue = prop.GetValue(other);

                if (thisValue is null && otherValue is null)
                {
                    continue;
                }

                if (thisValue is null || otherValue is null || thisValue.NotEquals(otherValue))
                {
                    return false;
                }
            }

            return true;
        }

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
        {
            PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int hash = 17;

            foreach (PropertyInfo prop in props)
            {
                hash = hash * 31 + (prop.GetValue(this)?.GetHashCode() ?? 0);
            }

            return hash;
        }

        /// <summary>
        /// Returns a string that represents the current object in JSON format.
        /// </summary>
        /// <returns>A JSON string representation of the current object.</returns>
        public override string ToString() => this.ToJson();
    }
}
