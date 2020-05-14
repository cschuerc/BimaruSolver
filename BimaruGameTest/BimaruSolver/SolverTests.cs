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
        [TestMethod]
        public void TestNoGridBackup()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();

            Assert.ThrowsException<ArgumentNullException>(() => new Solver(null, null, null, null));
        }

        [TestMethod]
        public void TestInvalidCounting()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var gridBackup = new Backup<IBimaruGrid>();

            new Solver(null, null, null, gridBackup, false);

            // No trial rule, so no counting
            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, null, gridBackup, true));

            
            var ruleNotDisjoint = new SingleTrivialChange();

            new Solver(null, null, ruleNotDisjoint, gridBackup, false);

            // No disjoint trial rule, so no counting
            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, ruleNotDisjoint, gridBackup, true));
        }

        [TestMethod]
        public void TestNoRules()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, null, new Backup<IBimaruGrid>());

            Assert.AreEqual(0, solver.Solve(game));
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestNoSolution()
        {
            var game = (new MockGameFactory()).GenerateGameNoSolution();

            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            Assert.AreEqual(0, solver.Solve(game));
            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestOneSolution()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();

            var preSolvingValues = new Dictionary<GridPoint, BimaruValue>();
            foreach (var p in game.Grid.AllPoints())
            {
                preSolvingValues[p] = game.Grid[p];
            }

            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(),true);
            Assert.AreEqual(1, solver.Solve(game));
            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestTwoSolutions()
        {
            var game = (new MockGameFactory()).GenerateGameTwoSolutions();
            var solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), true);
            Assert.AreEqual(2, solver.Solve(game));
            Assert.IsTrue(game.IsSolved);

            game = (new MockGameFactory()).GenerateGameTwoSolutions();
            solver = new Solver(null, null, new BruteForce(), new Backup<IBimaruGrid>(), false);
            Assert.AreEqual(1, solver.Solve(game));
            Assert.IsTrue(game.IsSolved);
        }

        private class ChangeLogger : IFieldValueChangedRule, ISolverRule
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
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipsPerLength[1] = 1;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|SW

            var changeLogger = new ChangeLogger();
            var solver = new Solver(new List<IFieldValueChangedRule>() { changeLogger }, null, null, new Backup<IBimaruGrid>());
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
            var game = (new MockGameFactory()).GenerateGameOneSolution();

            var solver = new Solver(null, null, null, new Backup<IBimaruGrid>());
            Assert.AreEqual(0, solver.Solve(game));

            // No InvalidFieldChange Exception here, although we change a field value back
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.UNDETERMINED;
        }

        [TestMethod]
        public void TestFieldChangedRules()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipsPerLength[1] = 1;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|S?

            var fieldChangedRule = new List<IFieldValueChangedRule>()
            {
                new SetShipEnvironment()
            };

            var solver = new Solver(fieldChangedRule, null, null, new Backup<IBimaruGrid>());
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestFullGridRules()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipsPerLength[1] = 1;
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_SINGLE;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|S?

            var fullGridRules = new List<ISolverRule>()
            {
                new FillRowOrColumnWithWater()
            };

            var solver = new Solver(null, fullGridRules, null, new Backup<IBimaruGrid>());
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
        }

        [TestMethod]
        public void TestFullGridOnceRule()
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

            Assert.AreEqual(1, changesOnce.CallCounter);
            Assert.IsTrue(changesUnlimited.CallCounter > 1);
        }

        [TestMethod]
        public void TestNonChanging()
        {
            var game = (new MockGameFactory()).GenerateGameOneSolution();
            var solver = new Solver(null, null, new SingleTrivialChange(), new Backup<IBimaruGrid>());

            // A non-changing trial and error rule could lead to an infinite recursion
            // => check if instead correct exception
            Assert.ThrowsException<InvalidOperationException>(() => solver.Solve(game));
        }

        /// <summary>
        /// A trial and error rule that produces a single trial
        /// with a single but trivial change.
        /// </summary>
        private class SingleTrivialChange : ITrialAndErrorRule
        {
            public bool AreTrialsDisjoint => false;

            public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
            {
                var p = new GridPoint(0, 0);
                BimaruValue oldValue = game.Grid[p];
                yield return new FieldsToChange<BimaruValue>(p, oldValue);
            }
        }
    }
}
