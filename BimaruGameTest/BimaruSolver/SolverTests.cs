using BimaruGame;
using BimaruInterfaces;
using BimaruTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        public void TestNoGridBackup()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Solver(null, null, null, null));
        }

        [TestMethod]
        public void TestNoTrialRule()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var gridBackup = new Backup<IBimaruGrid>();

            new Solver(null, null, null, gridBackup, false);

            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, null, gridBackup, true));
        }

        [TestMethod]
        public void TestIncompleteTrialRule()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var gridBackup = new Backup<IBimaruGrid>();
            var trialRule = new MockTrialRule(true, false);

            new Solver(null, null, trialRule, gridBackup, false);

            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, trialRule, gridBackup, true));
        }

        [TestMethod]
        public void TestNotDisjointTrialRule()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var gridBackup = new Backup<IBimaruGrid>();
            var trialRule = new MockTrialRule(false, true);

            new Solver(null, null, trialRule, gridBackup, false);

            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, trialRule, gridBackup, true));
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
                set;
            }

            public bool AreTrialsComplete
            {
                get;
                set;
            }

            public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IGame game)
            {
                var p = new GridPoint(0, 0);
                yield return new FieldsToChange<BimaruValue>(p, game.Grid[p]);
            }
        }

        [TestMethod]
        public void TestNoRules()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, null, new Backup<IBimaruGrid>());
            int numberOfSolutions = solver.Solve(game);

            Assert.AreEqual(0, numberOfSolutions);
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestNoSolution()
        {
            var game = (new MockGameFactory()).GenerateGameNoSolution();
            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            int numberOfSolutions = solver.Solve(game);

            Assert.AreEqual(0, numberOfSolutions);
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestNoSolutionNoGridOverwrite()
        {
            var game = (new MockGameFactory()).GenerateGameNoSolution();
            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            solver.Solve(game);

            var expectedGrid = (new MockGameFactory()).GenerateGameNoSolution().Grid;
            expectedGrid.AssertEqual(game.Grid);
        }

        [TestMethod]
        public void TestOneSolution()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            int numberOfSolutions = solver.Solve(game);

            Assert.AreEqual(1, numberOfSolutions);
            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestTwoSolutionsCounting()
        {
            var game = (new MockGameFactory()).GenerateGameTwoSolutions();
            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            int numberOfSolutions = solver.Solve(game);

            Assert.AreEqual(2, numberOfSolutions);
            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestTwoSolutionsNonCounting()
        {
            var game = (new MockGameFactory()).GenerateGameTwoSolutions();
            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), false);
            int numberOfSolutions = solver.Solve(game);

            Assert.AreEqual(1, numberOfSolutions);
            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestInitialFieldChanges()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;

            var changeLogger = new ChangeLogger();
            var solver = new Solver(new List<IFieldValueChangedRule>() { changeLogger }, null, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            AssertEqualChangedEventArgs(
                new List<FieldValueChangedEventArgs<BimaruValue>>()
                {
                    new FieldValueChangedEventArgs<BimaruValue>(new GridPoint(0, 0), BimaruValue.UNDETERMINED),
                    new FieldValueChangedEventArgs<BimaruValue>(new GridPoint(0, 1), BimaruValue.UNDETERMINED),
                },
                changeLogger);
        }

        private void AssertEqualChangedEventArgs(
            IEnumerable<FieldValueChangedEventArgs<BimaruValue>> expected,
            IEnumerable<FieldValueChangedEventArgs<BimaruValue>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            int index = 0;
            foreach (var e in expected)
            {
                Assert.AreEqual(e.Point, actual.ElementAt(index).Point);
                Assert.AreEqual(e.OriginalValue, actual.ElementAt(index).OriginalValue);
                index++;
            }
        }

        private class ChangeLogger : IFieldValueChangedRule, ISolverRule, IEnumerable<FieldValueChangedEventArgs<BimaruValue>>
        {
            public ChangeLogger(bool shallBeAppliedOnce = false)
            {
                ChangedEventArgs = new List<FieldValueChangedEventArgs<BimaruValue>>();
                NumberOfSolverRuleCalls = 0;

                ShallBeAppliedOnce = shallBeAppliedOnce;
            }

            private IList<FieldValueChangedEventArgs<BimaruValue>> ChangedEventArgs
            {
                get;
                set;
            }

            public int NumberOfSolverRuleCalls
            {
                get;
                private set;
            }

            public bool ShallBeAppliedOnce
            {
                get;
                private set;
            }

            public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
            {
                ChangedEventArgs.Add(e);
            }

            public void Solve(IGame game)
            {
                NumberOfSolverRuleCalls++;
            }

            public IEnumerator<FieldValueChangedEventArgs<BimaruValue>> GetEnumerator()
            {
                return ChangedEventArgs.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ChangedEventArgs.GetEnumerator();
            }
        }

        [TestMethod]
        public void TestCorrectUnsubscription()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            // No InvalidFieldChange exception here, although we change a field value back
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.UNDETERMINED;
        }

        [TestMethod]
        public void TestFieldChangedRules()
        {
            var game = GenerateEasyGame();

            var fieldChangedRule =
                new List<IFieldValueChangedRule>()
                {
                    new SetShipEnvironment()
                };

            var solver = new Solver(fieldChangedRule, null, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            Assert.IsTrue(game.IsSolved);
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
        private IGame GenerateEasyGame()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipsPerLength[1] = 1;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            return game;
        }

        [TestMethod]
        public void TestSolverRules()
        {
            var game = GenerateEasyGame();

            var fullGridRules = new List<ISolverRule>()
            {
                new FillRowOrColumnWithWater()
            };

            var solver = new Solver(null, fullGridRules, null, new Backup<IBimaruGrid>());
            solver.Solve(game);

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestSolverRuleOnce()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();

            var changesOnce = new ChangeLogger(true);
            var changesUnlimited = new ChangeLogger(false);
            var fullGridRules = new List<ISolverRule>()
            {
                changesOnce,
                changesUnlimited
            };

            var solver = new Solver(null, fullGridRules, new BruteForce(), new Backup<IBimaruGrid>());
            solver.Solve(game);

            Assert.IsTrue(changesOnce.NumberOfSolverRuleCalls == 1);
            Assert.IsTrue(changesUnlimited.NumberOfSolverRuleCalls > 1);
        }

        [TestMethod]
        public void TestOnlyTrivialChangeRule()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, new MockTrialRule(true, true), new Backup<IBimaruGrid>());

            // A non-changing trial and error rule could lead to an infinite recursion
            // => check if instead correct exception
            Assert.ThrowsException<InvalidOperationException>(() => solver.Solve(game));
        }
    }
}
