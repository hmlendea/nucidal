using System;

using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class DuplicateEntityExceptionTests
    {
        private static string EntityId => "angetenar";
        private static string EntityTypeName => nameof(TestEntityDataObject);

        [Test]
        public void Constructor_WithEntity_SetsEntityId()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void Constructor_WithEntity_SetsEntityTypeName()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntity_SetsMessage()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.Message, Does.Contain(EntityId));
            Assert.That(exception.Message, Does.Contain(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntity_MessageIndicatesDuplicated()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }

        [Test]
        public void Constructor_WithEntityIdAndType_SetsEntityId()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void Constructor_WithEntityIdAndType_SetsEntityTypeName()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityIdAndType_SetsMessage()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.Message, Does.Contain(EntityId));
            Assert.That(exception.Message, Does.Contain(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityIdAndStringType_SetsEntityId()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void Constructor_WithEntityIdAndStringType_SetsEntityTypeName()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityIdAndStringType_SetsMessage()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.Message, Does.Contain(EntityId));
            Assert.That(exception.Message, Does.Contain(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityAndInnerException_SetsEntityId()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void Constructor_WithEntityAndInnerException_SetsEntityTypeName()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityAndInnerException_SetsInnerException()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        [Test]
        public void Constructor_WithEntityAndInnerException_SetsMessage()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.Message, Does.Contain(EntityId));
        }

        [Test]
        public void Constructor_WithEntityIdTypeAndInnerException_SetsEntityId()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(
                EntityId,
                typeof(TestEntityDataObject),
                innerException);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void Constructor_WithEntityIdTypeAndInnerException_SetsEntityTypeName()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(
                EntityId,
                typeof(TestEntityDataObject),
                innerException);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityIdTypeAndInnerException_SetsInnerException()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(
                EntityId,
                typeof(TestEntityDataObject),
                innerException);

            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        [Test]
        public void Constructor_WithEntityIdStringTypeAndInnerException_SetsEntityId()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(EntityId, EntityTypeName, innerException);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void Constructor_WithEntityIdStringTypeAndInnerException_SetsEntityTypeName()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(EntityId, EntityTypeName, innerException);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void Constructor_WithEntityIdStringTypeAndInnerException_SetsInnerException()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(EntityId, EntityTypeName, innerException);

            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        [Test]
        public void IsEntityException()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception, Is.InstanceOf<EntityException>());
        }

        [Test]
        public void IsException()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception, Is.InstanceOf<Exception>());
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void Constructor_WithEntityIdAndType_EntityIdIsSetCorrectly(string entityId)
        {
            DuplicateEntityException exception = new(entityId, typeof(TestEntityDataObject));

            Assert.That(exception.EntityId, Is.EqualTo(entityId));
        }

        [Test]
        public void Constructor_WithEntityIdAndType_HasNullInnerException()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.InnerException, Is.Null);
        }

        [Test]
        public void Constructor_WithEntityIdAndStringType_MessageIndicatesDuplicated()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }

        [Test]
        public void Constructor_WithEntityIdAndType_MessageIndicatesDuplicated()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }
    }
}
