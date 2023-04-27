using System;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;
using Bimaru.Solver;
using Bimaru.Solver.CombinedRules;
using Bimaru.Solver.FieldChangedRules;
using Bimaru.Solver.TrialAndErrorRules;
using Bimaru.Utility;
using Xunit;

namespace Bimaru.Tests.Solver
{
    public class SolverTests
    {
        [Theory]
        [MemberData(nameof(CreateDataToTestCreation))]
        public void TestCreation(ITrialAndErrorRule trialRule, IBackup<IBimaruGrid> gridBackup, bool shallCountSolutions, Type expectedExceptionType)
        {
            var caughtException = Record.Exception(() => new BimaruSolver(null, null, trialRule, gridBackup, shallCountSolutions));

            Assert.Equal(expectedExceptionType, caughtException?.GetType());
        }

        public static IEnumerable<object[]> CreateDataToTestCreation()
        {
            // No grid Backup
            yield return new object[] { null, null, false, typeof(ArgumentNullException) };

            // Cannot count without trial rule
            yield return new object[] { null, new Backup<IBimaruGrid>(), true, typeof(ArgumentException) };

            // Cannot count with incomplete trial rule
            yield return new object[] { new MockTrialRule(true, false), new Backup<IBimaruGrid>(), true, typeof(ArgumentException) };

            // Cannot count with not disjoint trial rule
            yield return new object[] { new MockTrialRule(false, true), new Backup<IBimaruGrid>(), true, typeof(ArgumentException) };
        }

        /// <summary>
        /// Produces a single trial with a single but trivial change.
        /// </summary>
        private class MockTrialRule : ITrialAndErrorRule
        {
            public MockTrialRule(bool areTrialsDisjoint, bool areTrialsComplete)
            {
                AreTrialsDisjoint = areTrialsDisjoint;
                AreTrialsComplete = areTrialsComplete;
            }
            public bool AreTrialsDisjoint
            {
                get;
            }

            public bool AreTrialsComplete
            {
                get;
            }

            public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IBimaruGame game)
            {
                var p = new GridPoint(0, 0);
                yield return new FieldsToChange<BimaruValue>(p, game.Grid[p]);
            }
        }

        [Fact]
        public void TestNoRules()
        {
            var game = GameFactoryForTesting.GenerateGameOneSolution();
            var solver = new BimaruSolver(null, null, null, new Backup<IBimaruGrid>());
            var numberOfSolutions = solver.Solve(game);

            Assert.Equal(0, numberOfSolutions);
            Assert.False(game.IsSolved);
        }

        [Fact]
        public void TestNoSolution()
        {
            var game = GameFactoryForTesting.GenerateGameNoSolution();
            var solver = new BimaruSolver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            var numberOfSolutions = solver.Solve(game);

            Assert.Equal(0, numberOfSolutions);
            Assert.False(game.IsSolved);
        }

        [Fact]
        public void TestNoSolutionNoGridOverwrite()
        {
            var game = GameFactoryForTesting.GenerateGameNoSolution();
            var solver = new BimaruSolver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            solver.Solve(game);

            var expectedGrid = GameFactoryForTesting.GenerateGameNoSolution().Grid;
            expectedGrid.AssertEqual(game.Grid);
        }

        [Fact]
        public void TestOneSolution()
        {
            var game = GameFactoryForTesting.GenerateGameOneSolution();
            var solver = new BimaruSolver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            var numberOfSolutions = solver.Solve(game);

            Assert.Equal(1, numberOfSolutions);
            Assert.True(game.IsSolved);
        }

        [Fact]
        public void TestTwoSolutionsCounting()
        {
            var game = GameFactoryForTesting.GenerateGameTwoSolutions();
            var solver = new BimaruSolver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            var numberOfSolutions = solver.Solve(game);

            Assert.Equal(2, numberOfSolutions);
            Assert.True(game.IsSolved);
        }

        [Fact]
        public void TestTwoSolutionsNonCounting()
        {
            var game = GameFactoryForTesting.GenerateGameTwoSolutions();
            var solver = new BimaruSolver(null, null, new BruteForce(), new Backup<IBimaruGrid>());
            var numberOfSolutions = solver.Solve(game);

            Assert.Equal(1, numberOfSolutions);
            Assert.True(game.IsSolved);
        }

        [Fact]
        public void TestInitialFieldChanges()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 2);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;

            var changeLogger = new ChangeLogger();
            var solver = new BimaruSolver(new List<IFieldValueChangedRule>() { changeLogger }, null, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            AssertEqualChangedEventArgs(
                new List<FieldValueChangedEventArgs<BimaruValue>>()
                {
                    new(new GridPoint(0, 0), BimaruValue.UNDETERMINED),
                    new(new GridPoint(0, 1), BimaruValue.UNDETERMINED),
                },
                changeLogger.Changes);
        }

        private static void AssertEqualChangedEventArgs(
            IReadOnlyCollection<FieldValueChangedEventArgs<BimaruValue>> expected,
            IReadOnlyCollection<FieldValueChangedEventArgs<BimaruValue>> actual)
        {
            Assert.Equal(expected.Count, actual.Count);

            var index = 0;
            foreach (var e in expected)
            {
                Assert.Equal(e.Point, actual.ElementAt(index).Point);
                Assert.Equal(e.OriginalValue, actual.ElementAt(index).OriginalValue);
                index++;
            }
        }

        private class ChangeLogger : IFieldValueChangedRule, ISolverRule
        {
            public ChangeLogger(bool shallBeAppliedOnce = false)
            {
                ChangedEventArgs = new List<FieldValueChangedEventArgs<BimaruValue>>();
                NumberOfSolverRuleCalls = 0;

                ShallBeAppliedOnce = shallBeAppliedOnce;
            }

            private List<FieldValueChangedEventArgs<BimaruValue>> ChangedEventArgs
            {
                get;
            }

            public int NumberOfSolverRuleCalls
            {
                get;
                private set;
            }

            public bool ShallBeAppliedOnce
            {
                get;
            }

            public void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
            {
                ChangedEventArgs.Add(e);
            }

            public void Solve(IBimaruGame game)
            {
                NumberOfSolverRuleCalls++;
            }

            public IReadOnlyCollection<FieldValueChangedEventArgs<BimaruValue>> Changes => ChangedEventArgs;
        }

        [Fact]
        public void TestCorrectUnsubscription()
        {
            var game = GameFactoryForTesting.GenerateGameOneSolution();
            var solver = new BimaruSolver(null, null, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            // No InvalidFieldChange exception here, although we change a field value back
            Assert.Null(Record.Exception(() => game.Grid[new GridPoint(0, 0)] = BimaruValue.UNDETERMINED));
        }

        [Fact]
        public void TestFieldChangedRules()
        {
            var game = GenerateEasyGame();

            var fieldChangedRule =
                new List<IFieldValueChangedRule>()
                {
                    new SetShipEnvironment()
                };

            var solver = new BimaruSolver(fieldChangedRule, null, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            Assert.True(game.IsSolved);
        }

        /// <summary>
        /// 
        /// 1xSUBMARINE
        ///   10
        ///   --
        /// 0|??
        /// 1|S?
        /// 
        /// </summary>
        private static IBimaruGame GenerateEasyGame()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 2);
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipsPerLength[1] = 1;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            return game;
        }

        [Fact]
        public void TestSolverRules()
        {
            var game = GenerateEasyGame();

            var fullGridRules = new List<ISolverRule>()
            {
                new FillRowOrColumnWithWater()
            };

            var solver = new BimaruSolver(null, fullGridRules, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            Assert.True(game.IsSolved);
        }

        [Fact]
        public void TestSolverRuleOnce()
        {
            var game = GameFactoryForTesting.GenerateGameOneSolution();

            var changesOnce = new ChangeLogger(true);
            var changesUnlimited = new ChangeLogger(shallBeAppliedOnce: false);
            var fullGridRules = new List<ISolverRule>()
            {
                changesOnce,
                changesUnlimited
            };

            var solver = new BimaruSolver(null, fullGridRules, new BruteForce(), new Backup<IBimaruGrid>());
            solver.Solve(game);

            Assert.True(changesOnce.NumberOfSolverRuleCalls == 1);
            Assert.True(changesUnlimited.NumberOfSolverRuleCalls > 1);
        }

        [Fact]
        public void TestOnlyTrivialChangeRule()
        {
            var game = GameFactoryForTesting.GenerateGameOneSolution();
            var solver = new BimaruSolver(null, null, new MockTrialRule(true, true), new Backup<IBimaruGrid>());

            // A non-changing trial and error rule could lead to an infinite recursion
            // => check if instead correct exception
            Assert.Throws<InvalidOperationException>(() => solver.Solve(game));
        }
    }
}
