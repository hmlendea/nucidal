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

        // -- Constructor(EntityBase) ------

        [Test]
        public void GivenEntityArgument_WhenConstructed_ThenEntityIdIsSet()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void GivenEntityArgument_WhenConstructed_ThenEntityTypeNameIsSet()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void GivenEntityArgument_WhenConstructed_ThenMessageContainsEntityDetails()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.Message, Does.Contain(EntityId));
            Assert.That(exception.Message, Does.Contain(EntityTypeName));
        }

        [Test]
        public void GivenEntityArgument_WhenConstructed_ThenMessageIndicatesDuplicated()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }

        // -- Constructor(string, Type) ------

        [Test]
        public void GivenEntityIdAndTypeArguments_WhenConstructed_ThenEntityIdIsSet()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void GivenEntityIdAndTypeArguments_WhenConstructed_ThenEntityTypeNameIsSet()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void GivenEntityIdAndTypeArguments_WhenConstructed_ThenMessageContainsEntityDetails()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.Message, Does.Contain(EntityId));
            Assert.That(exception.Message, Does.Contain(EntityTypeName));
        }

        [Test]
        public void GivenEntityIdAndTypeArguments_WhenConstructed_ThenMessageIndicatesDuplicated()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }

        [Test]
        public void GivenEntityIdAndTypeArguments_WhenConstructed_ThenInnerExceptionIsNull()
        {
            DuplicateEntityException exception = new(EntityId, typeof(TestEntityDataObject));

            Assert.That(exception.InnerException, Is.Null);
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void GivenVariousEntityIds_WhenConstructedWithIdAndType_ThenEntityIdIsSetCorrectly(string entityId)
        {
            DuplicateEntityException exception = new(entityId, typeof(TestEntityDataObject));

            Assert.That(exception.EntityId, Is.EqualTo(entityId));
        }

        // -- Constructor(string, string) ------

        [Test]
        public void GivenEntityIdAndStringTypeArguments_WhenConstructed_ThenEntityIdIsSet()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void GivenEntityIdAndStringTypeArguments_WhenConstructed_ThenEntityTypeNameIsSet()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void GivenEntityIdAndStringTypeArguments_WhenConstructed_ThenMessageContainsEntityDetails()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.Message, Does.Contain(EntityId));
            Assert.That(exception.Message, Does.Contain(EntityTypeName));
        }

        [Test]
        public void GivenEntityIdAndStringTypeArguments_WhenConstructed_ThenMessageIndicatesDuplicated()
        {
            DuplicateEntityException exception = new(EntityId, EntityTypeName);

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }

        // -- Constructor(EntityBase, Exception) ------

        [Test]
        public void GivenEntityAndInnerExceptionArguments_WhenConstructed_ThenEntityIdIsSet()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void GivenEntityAndInnerExceptionArguments_WhenConstructed_ThenEntityTypeNameIsSet()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void GivenEntityAndInnerExceptionArguments_WhenConstructed_ThenInnerExceptionIsSet()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        [Test]
        public void GivenEntityAndInnerExceptionArguments_WhenConstructed_ThenMessageContainsEntityDetails()
        {
            TestEntityDataObject entity = new() { Id = EntityId };
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(entity, innerException);

            Assert.That(exception.Message, Does.Contain(EntityId));
        }

        // -- Constructor(string, Type, Exception) ------

        [Test]
        public void GivenEntityIdTypeAndInnerExceptionArguments_WhenConstructed_ThenEntityIdIsSet()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(
                EntityId,
                typeof(TestEntityDataObject),
                innerException);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void GivenEntityIdTypeAndInnerExceptionArguments_WhenConstructed_ThenEntityTypeNameIsSet()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(
                EntityId,
                typeof(TestEntityDataObject),
                innerException);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void GivenEntityIdTypeAndInnerExceptionArguments_WhenConstructed_ThenInnerExceptionIsSet()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(
                EntityId,
                typeof(TestEntityDataObject),
                innerException);

            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        // -- Constructor(string, string, Exception) ------

        [Test]
        public void GivenEntityIdStringTypeAndInnerExceptionArguments_WhenConstructed_ThenEntityIdIsSet()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(EntityId, EntityTypeName, innerException);

            Assert.That(exception.EntityId, Is.EqualTo(EntityId));
        }

        [Test]
        public void GivenEntityIdStringTypeAndInnerExceptionArguments_WhenConstructed_ThenEntityTypeNameIsSet()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(EntityId, EntityTypeName, innerException);

            Assert.That(exception.EntityTypeName, Is.EqualTo(EntityTypeName));
        }

        [Test]
        public void GivenEntityIdStringTypeAndInnerExceptionArguments_WhenConstructed_ThenInnerExceptionIsSet()
        {
            Exception innerException = new("inner");

            DuplicateEntityException exception = new(EntityId, EntityTypeName, innerException);

            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        // -- Type hierarchy ------

        [Test]
        public void GivenException_WhenCheckedForType_ThenIsEntityException()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception, Is.InstanceOf<EntityException>());
        }

        [Test]
        public void GivenException_WhenCheckedForType_ThenIsException()
        {
            TestEntityDataObject entity = new() { Id = EntityId };

            DuplicateEntityException exception = new(entity);

            Assert.That(exception, Is.InstanceOf<Exception>());
        }
    }
}
