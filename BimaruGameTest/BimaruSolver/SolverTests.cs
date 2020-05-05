using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class SolverTests
    {
        /// <summary>
        /// A trial and error rule that produces a single trial
        /// with a single but trivial change.
        /// </summary>
        private class SingleTrivialChange : ITrialAndErrorRule
        {
            /// <inheritdoc/>
            public bool AreTrialsDisjoint => false;

            /// <inheritdoc/>
            public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
            {
                var p = new GridPoint(0, 0);
                BimaruValue oldValue = game.Grid[p];
                yield return new FieldsToChange<BimaruValue>(p, oldValue);
            }
        }

        [TestMethod]
        public void TestInvalidCounting()
        {
            var game = (new GameFactory()).GenerateGameOneSolution();

            new Solver(null, null, null, false);

            // No trial rule, so no counting
            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, null, true));

            
            var ruleNotDisjoint = new SingleTrivialChange();

            new Solver(null, null, ruleNotDisjoint, false);

            // No disjoint trial rule, so no counting
            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, ruleNotDisjoint, true));
        }

        [TestMethod]
        public void TestNoRules()
        {
            var game = (new GameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, null);

            Assert.AreEqual(0, solver.Solve(game));
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestNoSolution()
        {
            var game = (new GameFactory()).GenerateGameNoSolution();

            var solver = new Solver(null, null, new BruteForce(), true);
            Assert.AreEqual(0, solver.Solve(game));
            Assert.IsFalse(game.IsSolved);

            // No solution => Only a single grid on the stack
            Assert.ThrowsException<InvalidOperationException>(() => game.Grid.Rollback());
        }

        [TestMethod]
        public void TestOneSolution()
        {
            var game = (new GameFactory()).GenerateGameOneSolution();

            var preSolvingValues = new Dictionary<GridPoint, BimaruValue>();
            foreach (var p in game.Grid.AllPoints())
            {
                preSolvingValues[p] = game.Grid[p];
            }

            var solver = new Solver(null, null, new BruteForce(), true);
            Assert.AreEqual(1, solver.Solve(game));
            Assert.IsTrue(game.IsSolved);

            game.Grid.Rollback();
            Assert.IsFalse(game.IsSolved);
            foreach (var p in game.Grid.AllPoints())
            {
                Assert.AreEqual(preSolvingValues[p], game.Grid[p]);
            }

            // Check no more grids on the stack
            Assert.ThrowsException<InvalidOperationException>(() => game.Grid.Rollback());
        }

        [TestMethod]
        public void TestTwoSolutions()
        {
            var game = (new GameFactory()).GenerateGameTwoSolutions();
            var solver = new Solver(null, null, new BruteForce(), true);
            Assert.AreEqual(2, solver.Solve(game));
            Assert.IsTrue(game.IsSolved);

            game = (new GameFactory()).GenerateGameTwoSolutions();
            solver = new Solver(null, null, new BruteForce(), false);
            Assert.AreEqual(1, solver.Solve(game));
            Assert.IsTrue(game.IsSolved);
        }

        private class ChangeLogger : IFieldChangedRule, IFullGridRule
        {
            public ChangeLogger(bool shallBeAppliedOnce = false)
            {
                Changes = new List<FieldValueChangedEventArgs<BimaruValue>>();
                CallCounter = 0;

                ShallBeAppliedOnce = shallBeAppliedOnce;
            }

            public int CallCounter { get; private set; }

            public IList<FieldValueChangedEventArgs<BimaruValue>> Changes { get; private set; }

            public bool ShallBeAppliedOnce { get; private set; }

            public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
            {
                Changes.Add(e);
                CallCounter++;
            }

            public void Solve(IGame game)
            {
                CallCounter++;
            }
        }

        [TestMethod]
        public void TestInitialFieldChanges()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|SW

            var changeLogger = new ChangeLogger();
            var solver = new Solver(new List<IFieldChangedRule>() { changeLogger }, null, null);
            solver.Solve(game);

            Assert.AreEqual(2, changeLogger.Changes.Count);
            Assert.AreEqual(BimaruValue.UNDETERMINED, changeLogger.Changes[0].OriginalValue);
            Assert.AreEqual(new GridPoint(0, 0), changeLogger.Changes[0].Point);
            Assert.AreEqual(BimaruValue.UNDETERMINED, changeLogger.Changes[1].OriginalValue);
            Assert.AreEqual(new GridPoint(0, 1), changeLogger.Changes[1].Point);
        }

        [TestMethod]
        public void TestCorrectUnsubscription()
        {
            var game = (new GameFactory()).GenerateGameOneSolution();

            var solver = new Solver(null, null, null);
            Assert.AreEqual(0, solver.Solve(game));

            // No InvalidFieldChange Exception here, although we change a field value back
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.UNDETERMINED;
        }

        [TestMethod]
        public void TestFieldChangedRules()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|S?

            var fieldChangedRule = new List<IFieldChangedRule>()
            {
                new SetShipEnvironment()
            };

            var solver = new Solver(fieldChangedRule, null, null);
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestFullGridRules()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|S?

            var fullGridRules = new List<IFullGridRule>()
            {
                new FillRowOrColumnWithWater()
            };

            var solver = new Solver(null, fullGridRules, null);
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestFullGridOnceRule()
        {
            var game = (new GameFactory()).GenerateGameOneSolution();

            var changesOnce = new ChangeLogger(true);
            var changesUnlimited = new ChangeLogger(false);
            var fullGridRules = new List<IFullGridRule>()
            {
                changesOnce,
                changesUnlimited
            };

            var solver = new Solver(null, fullGridRules, new BruteForce());
            solver.Solve(game);

            Assert.AreEqual(1, changesOnce.CallCounter);
            Assert.IsTrue(changesUnlimited.CallCounter > 1);
        }

        [TestMethod]
        public void TestNonChanging()
        {
            var game = (new GameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, new SingleTrivialChange());

            // A non-changing trial and error rule could lead to an infinite recursion
            // => check if instead correct exception
            Assert.ThrowsException<InvalidOperationException>(() => solver.Solve(game));
        }
    }
}
