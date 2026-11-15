using System;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Repository exception.
    /// </summary>
    public class EntityNotFoundException : EntityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityNotFoundException(EntityBase entity)
            : base(entity, BuildEntityMessage(entity.Id, entity.GetType().Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public EntityNotFoundException(string entityId, Type entityType)
            : base(entityId, entityType, BuildEntityMessage(entityId, entityType.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        public EntityNotFoundException(Type entityType)
            : base(string.Empty, entityType, $"The requested {entityType.Name} entity can not be found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        public EntityNotFoundException(string entityId, string entityType)
            : base(entityId, entityType, BuildEntityMessage(entityId, entityType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityNotFoundException(EntityBase entity, Exception innerException)
            : base(entity, BuildEntityMessage(entity.Id, entity.GetType().Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityNotFoundException(string entityId, Type entityType, Exception innerException)
            : base(entityId, entityType, BuildEntityMessage(entityId, entityType.Name), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> exception.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <param name="entityType">Entity type.</param>
        /// <param name="innerException">Inner exception.</param>
        public EntityNotFoundException(string entityId, string entityType, Exception innerException)
            : base(entityId, entityType, BuildEntityMessage(entityId, entityType), innerException)
        {
        }

        private static string BuildEntityMessage(string entityId, string entityType)
            => $"The {entityId} {entityType} entity can not be found.";
    }
}
