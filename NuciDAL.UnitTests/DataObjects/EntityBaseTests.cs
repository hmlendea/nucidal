using NUnit.Framework;

using NuciDAL.DataObjects;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.DataObjects
{
    [TestFixture]
    public sealed class EntityBaseTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        [Test]
        public void Equals_WhenComparingToNull_ReturnsFalse()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity.Equals(null);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenComparingToSameReference_ReturnsTrue()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity.Equals(entity);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WhenEntitiesHaveIdenticalProperties_ReturnsTrue()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WhenEntitiesHaveIdenticalProperties_IsSymmetric()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            Assert.That(entity1.Equals(entity2), Is.EqualTo(entity2.Equals(entity1)));
        }

        [Test]
        public void Equals_WhenEntitiesHaveDifferentIds_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = SecondEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenEntitiesHaveDifferentNames_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenEntitiesHaveDifferentValues_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = SecondEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenComparingToDifferentEntityType_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            AnotherTestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenNameIsNullOnFirstEntityOnly_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = null,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenNameIsNullOnSecondEntityOnly_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = null,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WhenNameIsNullOnBothEntities_ReturnsTrue()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = null,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = null,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WhenIdIsNullOnBothEntitiesAndOtherPropertiesMatch_ReturnsTrue()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = null,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = null,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WhenAllPropertiesAreNull_ReturnsTrue()
        {
            TestEntityDataObject entity1 = new() { Id = null, Name = null };
            TestEntityDataObject entity2 = new() { Id = null, Name = null };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ObjectEquals_WhenComparingToNull_ReturnsFalse()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity.Equals((object)null);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ObjectEquals_WhenComparingToNonEntityObject_ReturnsFalse()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity.Equals((object)"not-an-entity");

            Assert.That(result, Is.False);
        }

        [Test]
        public void ObjectEquals_WhenComparingToSameReference_ReturnsTrue()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity.Equals((object)entity);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ObjectEquals_WhenComparingToEntityWithIdenticalProperties_ReturnsTrue()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals((object)entity2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ObjectEquals_WhenComparingToEntityWithDifferentId_ReturnsFalse()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = SecondEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            bool result = entity1.Equals((object)entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GetHashCode_WhenEntitiesAreEqual_ReturnsSameHashCode()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            Assert.That(entity1.GetHashCode(), Is.EqualTo(entity2.GetHashCode()));
        }

        [Test]
        public void GetHashCode_WhenCalledMultipleTimes_ReturnsSameValue()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            int firstHash = entity.GetHashCode();
            int secondHash = entity.GetHashCode();

            Assert.That(secondHash, Is.EqualTo(firstHash));
        }

        [Test]
        public void GetHashCode_WhenAllPropertiesAreNull_DoesNotThrow()
        {
            TestEntityDataObject entity = new() { Id = null, Name = null };

            Assert.That(() => entity.GetHashCode(), Throws.Nothing);
        }

        [Test]
        public void GetHashCode_WhenEntitiesHaveDifferentIds_ReturnsDifferentHashCodes()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = SecondEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            Assert.That(entity1.GetHashCode(), Is.Not.EqualTo(entity2.GetHashCode()));
        }

        [Test]
        public void GetHashCode_WhenEntitiesHaveDifferentNames_ReturnsDifferentHashCodes()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = FirstEntityValue,
            };

            Assert.That(entity1.GetHashCode(), Is.Not.EqualTo(entity2.GetHashCode()));
        }

        [Test]
        public void ToString_ReturnsNonNullString()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            string result = entity.ToString();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ToString_ReturnsNonEmptyString()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            string result = entity.ToString();

            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void ToString_ReturnsStringContainingEntityId()
        {
            TestEntityDataObject entity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            string result = entity.ToString();

            Assert.That(result, Does.Contain(FirstEntityId));
        }

        [Test]
        public void ToString_WhenCalledOnDifferentEntities_ReturnsDifferentStrings()
        {
            TestEntityDataObject entity1 = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };
            TestEntityDataObject entity2 = new()
            {
                Id = SecondEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            };

            Assert.That(entity1.ToString(), Is.Not.EqualTo(entity2.ToString()));
        }

        [Test]
        public void Equals_WithIntKey_WhenEntitiesHaveIdenticalProperties_ReturnsTrue()
        {
            IntKeyEntityDataObject entity1 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };
            IntKeyEntityDataObject entity2 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WithIntKey_WhenEntitiesHaveDifferentIds_ReturnsFalse()
        {
            IntKeyEntityDataObject entity1 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };
            IntKeyEntityDataObject entity2 = new()
            {
                Id = SecondEntityValue,
                Name = FirstEntityName,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WithIntKey_WhenComparingToNull_ReturnsFalse()
        {
            IntKeyEntityDataObject entity = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };

            bool result = entity.Equals(null);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_WithIntKey_WhenComparingToSameReference_ReturnsTrue()
        {
            IntKeyEntityDataObject entity = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };

            bool result = entity.Equals(entity);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_WithIntKey_WhenEntitiesHaveDifferentNames_ReturnsFalse()
        {
            IntKeyEntityDataObject entity1 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };
            IntKeyEntityDataObject entity2 = new()
            {
                Id = FirstEntityValue,
                Name = SecondEntityName,
            };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GetHashCode_WithIntKey_WhenEntitiesAreEqual_ReturnsSameHashCode()
        {
            IntKeyEntityDataObject entity1 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };
            IntKeyEntityDataObject entity2 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };

            Assert.That(entity1.GetHashCode(), Is.EqualTo(entity2.GetHashCode()));
        }

        [Test]
        public void GetHashCode_WithIntKey_WhenEntitiesHaveDifferentIds_ReturnsDifferentHashCodes()
        {
            IntKeyEntityDataObject entity1 = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };
            IntKeyEntityDataObject entity2 = new()
            {
                Id = SecondEntityValue,
                Name = FirstEntityName,
            };

            Assert.That(entity1.GetHashCode(), Is.Not.EqualTo(entity2.GetHashCode()));
        }
    }
}
