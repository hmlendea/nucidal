using System;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Duplicate entity exception.
    /// </summary>
    public class DuplicateEntityException : EntityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public DuplicateEntityException(EntityBase entity)
            : base(entity, BuildMessage(entity.Id, entity.GetType().Name))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public DuplicateEntityException(string entityId, Type entityType)
            : base(entityId, entityType, BuildMessage(entityId, entityType.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public DuplicateEntityException(string entityId, string entityType)
            : base(entityId, entityType, BuildMessage(entityId, entityType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="innerException">Inner exception.</param>
        public DuplicateEntityException(EntityBase entity, Exception innerException)
            : base(entity, BuildMessage(entity.Id, entity.GetType().Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public DuplicateEntityException(string entityId, Type entityType, Exception innerException)
            : base(entityId, entityType, BuildMessage(entityId, entityType.Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateEntityException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public DuplicateEntityException(string entityId, string entityType, Exception innerException)
            : base(entityId, entityType, BuildMessage(entityId, entityType), innerException)
        {
        }

        private static string BuildMessage(string entityId, string entityType)
            => $"The {entityId} {entityType} entity is duplicated.";
    }
}
