using System;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Repository exception.
    /// </summary>
    public class EntityAlreadyExistsException : EntityException
    {
        private static string DefaultMessageFormat => "The {0} {1} entity already exists.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityAlreadyExistsException(EntityBase entity)
            : base(entity, string.Format(DefaultMessageFormat, entity.Id, entity.GetType().Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public EntityAlreadyExistsException(string entityId, Type entityType)
            : base(entityId, entityType, string.Format(DefaultMessageFormat, entityId, entityType.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public EntityAlreadyExistsException(string entityId, string entityType)
            : base(entityId, entityType, string.Format(DefaultMessageFormat, entityId, entityType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityAlreadyExistsException(EntityBase entity, Exception innerException)
            : base(entity, string.Format(DefaultMessageFormat, entity.Id, entity.GetType().Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityAlreadyExistsException(string entityId, Type entityType, Exception innerException)
            : base(entityId, entityType, string.Format(DefaultMessageFormat, entityId, entityType.Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityAlreadyExistsException(string entityId, string entityType, Exception innerException)
            : base(entityId, entityType, string.Format(DefaultMessageFormat, entityId, entityType), innerException)
        {
        }
    }
}
