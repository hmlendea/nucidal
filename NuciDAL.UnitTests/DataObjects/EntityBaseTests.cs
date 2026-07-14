using NUnit.Framework;
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

        // -- Equals ------

        [Test]
        public void GivenNullComparison_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenSameReference_WhenEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenIdenticalEntities_WhenEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenIdenticalEntities_WhenEqualsIsCalled_ThenIsSymmetric()
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
        public void GivenEntitiesWithDifferentIds_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenEntitiesWithDifferentNames_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenEntitiesWithDifferentValues_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenDifferentEntityType_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenNameNullOnFirstEntityOnly_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenNameNullOnSecondEntityOnly_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenNameNullOnBothEntities_WhenEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenIdNullOnBothEntitiesWithMatchingProperties_WhenEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenAllPropertiesNull_WhenEqualsIsCalled_ThenReturnsTrue()
        {
            TestEntityDataObject entity1 = new() { Id = null, Name = null };
            TestEntityDataObject entity2 = new() { Id = null, Name = null };

            bool result = entity1.Equals(entity2);

            Assert.That(result, Is.True);
        }

        // -- Equals (object overload) ------

        [Test]
        public void GivenNullObjectComparison_WhenObjectEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenNonEntityObjectComparison_WhenObjectEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenSameReferenceObjectComparison_WhenObjectEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenEntityWithIdenticalProperties_WhenObjectEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenEntityWithDifferentId_WhenObjectEqualsIsCalled_ThenReturnsFalse()
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

        // -- GetHashCode ------

        [Test]
        public void GivenEqualEntities_WhenGetHashCodeIsCalled_ThenReturnsSameHashCode()
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
        public void GivenSingleEntity_WhenGetHashCodeIsCalledMultipleTimes_ThenReturnsSameValue()
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
        public void GivenAllPropertiesNull_WhenGetHashCodeIsCalled_ThenDoesNotThrow()
        {
            TestEntityDataObject entity = new() { Id = null, Name = null };

            Assert.That(() => entity.GetHashCode(), Throws.Nothing);
        }

        [Test]
        public void GivenEntitiesWithDifferentIds_WhenGetHashCodeIsCalled_ThenReturnsDifferentHashCodes()
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
        public void GivenEntitiesWithDifferentNames_WhenGetHashCodeIsCalled_ThenReturnsDifferentHashCodes()
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

        // -- ToString ------

        [Test]
        public void GivenEntity_WhenToStringIsCalled_ThenReturnsNonNullString()
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
        public void GivenEntity_WhenToStringIsCalled_ThenReturnsNonEmptyString()
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
        public void GivenEntity_WhenToStringIsCalled_ThenReturnsStringContainingEntityId()
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
        public void GivenDifferentEntities_WhenToStringIsCalled_ThenReturnsDifferentStrings()
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

        // -- Equals (int key) ------

        [Test]
        public void GivenIntKeyEntitiesWithIdenticalProperties_WhenEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenIntKeyEntitiesWithDifferentIds_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenIntKeyNullComparison_WhenEqualsIsCalled_ThenReturnsFalse()
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
        public void GivenIntKeySameReference_WhenEqualsIsCalled_ThenReturnsTrue()
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
        public void GivenIntKeyEntitiesWithDifferentNames_WhenEqualsIsCalled_ThenReturnsFalse()
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

        // -- GetHashCode (int key) ------

        [Test]
        public void GivenEqualIntKeyEntities_WhenGetHashCodeIsCalled_ThenReturnsSameHashCode()
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
        public void GivenIntKeyEntitiesWithDifferentIds_WhenGetHashCodeIsCalled_ThenReturnsDifferentHashCodes()
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
