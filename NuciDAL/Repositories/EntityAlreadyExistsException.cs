using System;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Repository exception.
    /// </summary>
    public class EntityAlreadyExistsException : EntityException
    {
        private static string BuildMessage(string entityId, string entityType)
            => $"The '{entityId}' {entityType} already exists.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityAlreadyExistsException(EntityBase entity)
            : base(entity, BuildMessage(entity.Id, entity.GetType().Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public EntityAlreadyExistsException(string entityId, Type entityType)
            : base(entityId, entityType, BuildMessage(entityId, entityType.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public EntityAlreadyExistsException(string entityId, string entityType)
            : base(entityId, entityType, BuildMessage(entityId, entityType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityAlreadyExistsException(EntityBase entity, Exception innerException)
            : base(entity, BuildMessage(entity.Id, entity.GetType().Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityAlreadyExistsException(string entityId, Type entityType, Exception innerException)
            : base(entityId, entityType, BuildMessage(entityId, entityType.Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityAlreadyExistsException(string entityId, string entityType, Exception innerException)
            : base(entityId, entityType, BuildMessage(entityId, entityType), innerException)
        {
        }
    }
}
