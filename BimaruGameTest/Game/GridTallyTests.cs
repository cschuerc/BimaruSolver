using System;
using Bimaru.Game;
using Bimaru.Interface;
using Xunit;

// ReSharper disable RedundantAssignment

namespace Bimaru.Test
{
    public class GridTallyTests
    {
        [Theory]
        [InlineData(-100, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, typeof(ArgumentOutOfRangeException))]
        [InlineData(1, null)]
        [InlineData(2, null)]
        [InlineData(10, null)]
        public void TestCreation(int tallyLength, Type expectedExceptionType)
        {
            var exceptionCaught = Record.Exception(() => new GridTally(tallyLength));

            Assert.Equal(expectedExceptionType, exceptionCaught?.GetType());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public void TestLength(int tallyLength)
        {
            var tally = new GridTally(tallyLength);

            Assert.Equal(tallyLength, tally.Length);
        }

        [Theory]
        [InlineData(3, -10, typeof(IndexOutOfRangeException))]
        [InlineData(3, -2, typeof(IndexOutOfRangeException))]
        [InlineData(3, -1, typeof(IndexOutOfRangeException))]
        [InlineData(3, 0, null)]
        [InlineData(3, 2, null)]
        [InlineData(3, 3, typeof(IndexOutOfRangeException))]
        [InlineData(3, 4, typeof(IndexOutOfRangeException))]
        [InlineData(3, 13, typeof(IndexOutOfRangeException))]
        public void TestIndex(int tallyLength, int index, Type expectedExceptionType)
        {
            var tally = new GridTally(tallyLength);
            var exceptionCaught = Record.Exception(() => tally[index]);

            Assert.Equal(expectedExceptionType, exceptionCaught?.GetType());
        }

        [Fact]
        public void TestDefaultValueZero()
        {
            var tally = new GridTally(3);

            foreach (var t in tally)
            {
                Assert.Equal(0, t);
            }
        }

        [Theory]
        [InlineData(-10, typeof(ArgumentOutOfRangeException))]
        [InlineData(-2, typeof(ArgumentOutOfRangeException))]
        [InlineData(-1, typeof(ArgumentOutOfRangeException))]
        [InlineData(0, null)]
        [InlineData(1, null)]
        [InlineData(10, null)]
        public void TestValueRange(int value, Type expectedExceptionType)
        {
            var tally = new GridTally(3);
            var exceptionCaught = Record.Exception(() => tally[1] = value);

            Assert.Equal(expectedExceptionType, exceptionCaught?.GetType());
        }

        [Fact]
        public void TestValueSet()
        {
            var tally = new GridTally(3)
            {
                [0] = 7,
                [1] = 5,
                [2] = 0
            };

            Assert.Equal(7, tally[0]);
            Assert.Equal(5, tally[1]);
            Assert.Equal(0, tally[2]);
        }

        [Fact]
        public void TestTotal()
        {
            var tally = new GridTally(3);

            Assert.Equal(0, tally.Total);

            tally[0] = 7;

            Assert.Equal(7, tally.Total);

            tally[1] = 5;

            Assert.Equal(12, tally.Total);

            tally[2] = 0;

            Assert.Equal(12, tally.Total);
        }

        [Fact]
        public void TestSatisfiabilityLengthRange()
        {
            var tally = new GridTally(3);

            Assert.Throws<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[2], new int[2]));
            Assert.Throws<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[2], new int[3]));
            Assert.Throws<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[3], new int[2]));
            tally.GetSatisfiability(new int[3], new int[3]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[3], new int[4]));
            Assert.Throws<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[4], new int[3]));
            Assert.Throws<ArgumentOutOfRangeException>(() => tally.GetSatisfiability(new int[4], new int[4]));
        }

        [Fact]
        public void TestSatisfiabilityNoAdditional()
        {
            var tally = new GridTally(1)
            {
                [0] = 3
            };

            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 1 }, new[] { 0 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 2 }, new[] { 0 }));
            Assert.Equal(Satisfiability.SATISFIED, tally.GetSatisfiability(new[] { 3 }, new[] { 0 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 4 }, new[] { 0 }));
        }

        [Fact]
        public void TestSatisfiabilityAdditional()
        {
            var tally = new GridTally(1)
            {
                [0] = 3
            };

            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 0 }, new[] { 1 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 0 }, new[] { 2 }));
            Assert.Equal(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new[] { 0 }, new[] { 3 }));
            Assert.Equal(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new[] { 0 }, new[] { 4 }));
        }

        [Fact]
        public void TestSatisfiabilityCombination()
        {
            var tally = new GridTally(1)
            {
                [0] = 3
            };

            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 1 }, new[] { 1 }));
            Assert.Equal(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new[] { 1 }, new[] { 2 }));
            Assert.Equal(Satisfiability.SATISFIED, tally.GetSatisfiability(new[] { 3 }, new[] { 0 }));
            Assert.Equal(Satisfiability.SATISFIED, tally.GetSatisfiability(new[] { 3 }, new[] { 1 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 4 }, new[] { 0 }));
        }

        [Fact]
        public void TestSatisfiabilityMoreThanOne()
        {
            var tally = new GridTally(2)
            {
                [0] = 1,
                [1] = 2
            };

            Assert.Equal(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new[] { 0, 0 }, new[] { 1, 2 }));
            Assert.Equal(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new[] { 1, 0 }, new[] { 0, 2 }));
            Assert.Equal(Satisfiability.SATISFIABLE, tally.GetSatisfiability(new[] { 0, 2 }, new[] { 1, 0 }));

            Assert.Equal(Satisfiability.SATISFIED, tally.GetSatisfiability(new[] { 1, 2 }, new[] { 0, 0 }));

            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 2, 1 }, new[] { 0, 1 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 0, 3 }, new[] { 1, 0 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 2, 2 }, new[] { 0, 0 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 1, 3 }, new[] { 0, 0 }));
            Assert.Equal(Satisfiability.VIOLATED, tally.GetSatisfiability(new[] { 2, 3 }, new[] { 0, 0 }));
        }

        [Fact]
        public void TestEnumerator()
        {
            var tally = new GridTally(3)
            {
                [0] = 7,
                [1] = 5,
                [2] = 0
            };

            Assert.Equal(tally.Length, tally.Length);

            var index = 0;
            foreach (var t in tally)
            {
                Assert.Equal(tally[index], t);

                index++;
            }
        }
    }
}
