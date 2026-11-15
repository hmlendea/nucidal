using System;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Entity exception.
    /// </summary>
    public abstract class EntityException : Exception
    {
        /// <summary>
        /// Entity identifier.
        /// </summary>
        public string EntityId { get; }

        /// <summary>
        /// Entity type name.
        /// </summary>
        public string EntityTypeName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The exception message.</param>
        public EntityException(
            EntityBase entity,
            string message)
            : this(entity.Id, entity.GetType().Name, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="message">The exception message.</param>
        public EntityException(
            string entityId,
            Type entityType,
            string message)
            : this(entityId, entityType.Name, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="message">The exception message.</param>
        public EntityException(
            string entityId,
            string entityType,
            string message)
            : base(message)
        {
            EntityId = entityId;
            EntityTypeName = entityType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityException(
            EntityBase entity,
            string message,
            Exception innerException)
            : this(entity.Id, entity.GetType().Name, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityException(
            string entityId,
            Type entityType,
            string message,
            Exception innerException)
            : this(entityId, entityType.Name, message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityException(
            string entityId,
            string entityType,
            string message,
            Exception innerException)
            : base(message, innerException)
        {
            EntityId = entityId;
            EntityTypeName = entityType;
        }
    }
}
