using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Utility
{
    public class FieldsToChangeTests
    {
        [Fact]
        public void TestNoChanges()
        {
            var changes = new FieldsToChange<int>();

            AssertEqualFieldChanges(new List<SingleChange<int>>(), changes);
        }

        private static void AssertEqualFieldChanges(IReadOnlyCollection<SingleChange<int>> expectedFieldChanges, FieldsToChange<int> actualFieldChanges)
        {
            Assert.Equal(expectedFieldChanges.Count, actualFieldChanges.Count);

            foreach (var actualChange in actualFieldChanges)
            {
                Assert.Contains(actualChange, expectedFieldChanges);
            }
        }

        [Fact]
        public void TestSingleChange()
        {
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>(p12, 7);

            AssertEqualFieldChanges(
                new List<SingleChange<int>>()
                {
                    new(p12, 7)
                },
                changes);
        }

        [Fact]
        public void TestAdd()
        {
            var p12 = new GridPoint(1, 2);
            var p14 = new GridPoint(1, 4);
            var changes = new FieldsToChange<int>
            {
                { p12, 7 },
                { p14, 8 }
            };

            AssertEqualFieldChanges(
                new List<SingleChange<int>>()
                {
                    new(p12, 7),
                    new(p14, 8)
                },
                changes);
        }

        [Fact]
        public void TestSeveralChanges()
        {
            const int numValues = 2;
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>(
                p12,
                Direction.RIGHT,
                Enumerable.Range(7, numValues));

            AssertEqualFieldChanges(
                new List<SingleChange<int>>()
                {
                    new(p12, 7),
                    new(new GridPoint(1, 3), 8)
                },
                changes);
        }

        [Fact]
        public void TestOverwrite()
        {
            var p12 = new GridPoint(1, 2);
            var changes = new FieldsToChange<int>
            {
                { p12, 7 },
                { p12, 9 },
                { p12, 1 },
            };

            AssertEqualFieldChanges(
                new List<SingleChange<int>>()
                {
                    new(p12, 1),
                },
                changes);
        }
    }
}
