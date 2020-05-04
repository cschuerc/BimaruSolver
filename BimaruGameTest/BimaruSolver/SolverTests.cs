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
        private static Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            RollbackGrid grid = new RollbackGrid(numRows, numColumns);

            return new Game(rowTally, columnTally, settings, grid);
        }

        [TestMethod]
        public void TestNullArguments()
        {
            int numRows = 4;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            var solver = new Solver(null, null, null);
            solver.Solve(game);
        }

        [TestMethod]
        public void TestInvalidCounting()
        {
            int numRows = 4;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, null, true));

            var ruleNotDisjoint = new SingleTrivialChange();
            Assert.ThrowsException<ArgumentException>(() => new Solver(null, null, ruleNotDisjoint, true));
        }

        [TestMethod]
        public void TestInvalidGame()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;

            var solver = new Solver(null, null, null);

            Assert.AreEqual(0, solver.Solve(game));

            Assert.IsFalse(game.IsSolved);
        }

        [TestMethod]
        public void TestTrialAndErrorRuleSuccess()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            var solver = new Solver(null, null, new BruteForce(), true);
            Assert.AreEqual(1, solver.Solve(game));

            //   00
            //   --
            // 0|WW
            // 0|WW

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            game = SetupGame(numRows, numColumns);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.SHIP_UNDETERMINED);
            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.ColumnTally[1] = 2;
            game.ShipSettings[2] = 1;

            // 1xDESTROYER
            //   02
            //   --
            // 1|WS
            // 1|WS

            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_UP, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.SHIP_CONT_DOWN, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            game.Grid.Rollback();
            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.SHIP_UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            // Check no more grids on the stack
            Assert.ThrowsException<InvalidOperationException>(() => game.Grid.Rollback());
        }

        [TestMethod]
        public void TestTrialAndErrorRuleFail()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.ColumnTally[0] = 1;
            game.ColumnTally[1] = 1;
            game.ShipSettings[1] = 2;

            // 2xSUBMARINE
            //   11
            //   --
            // 1|SW
            // 1|WS
            // -> No solution

            var solver = new Solver(null, null, new BruteForce(), true);
            Assert.AreEqual(0, solver.Solve(game));

            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            // No solution => Only a single grid on the stack
            Assert.ThrowsException<InvalidOperationException>(() => game.Grid.Rollback());
        }

        private class ChangeLogger : IFieldChangedRule, IFullGridRule
        {
            public ChangeLogger(bool shallBeAppliedOnce = false)
            {
                Changes = new List<FieldValueChangedEventArgs<BimaruValue>>();
                CallCounter = 0;

                ShallBeAppliedOnce = shallBeAppliedOnce;
            }

            public int CallCounter
            {
                get;
                private set;
            }

            public IList<FieldValueChangedEventArgs<BimaruValue>> Changes
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
                Changes.Add(e);
                CallCounter++;
            }

            public void Solve(IGame game)
            {
                CallCounter++;
            }
        }

        [TestMethod]
        public void TestInitialConditions()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            game.Grid.SetFieldValue(new GridPoint(0, 1), BimaruValue.WATER);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   01
            //   --
            // 0|??
            // 1|SW

            var changeLogger = new ChangeLogger();
            var solver = new Solver(new List<IFieldChangedRule>() { changeLogger }, null, null);
            Assert.AreEqual(0, solver.Solve(game));

            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(BimaruValue.SHIP_SINGLE, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            Assert.AreEqual(2, changeLogger.Changes.Count);

            Assert.AreEqual(BimaruValue.UNDETERMINED, changeLogger.Changes[0].OriginalValue);
            Assert.AreEqual(new GridPoint(0, 0), changeLogger.Changes[0].Point);
            Assert.AreEqual(BimaruValue.UNDETERMINED, changeLogger.Changes[1].OriginalValue);
            Assert.AreEqual(new GridPoint(0, 1), changeLogger.Changes[1].Point);
        }

        [TestMethod]
        public void TestCorrectUnsubscription()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|??

            var solver = new Solver(null, null, null);
            Assert.AreEqual(0, solver.Solve(game));

            // No InvalidFieldChange Exception here, although we change a field value back
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.UNDETERMINED);
        }

        [TestMethod]
        public void TestFullGridAndChangedRules()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|??

            var fieldChangedRules = new List<IFieldChangedRule>() { new WaterAroundSingleShip() };
            var fullGridRules = new List<IFullGridRule>() { new OneShipRowCol() };
            var solver = new Solver(fieldChangedRules, fullGridRules, null);
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(BimaruValue.SHIP_SINGLE, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));
        }

        [TestMethod]
        public void TestOnceRule()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|??

            var changesOnce = new ChangeLogger(true);
            var changesUnlimited = new ChangeLogger(false);
            var solver = new Solver(null, new List<IFullGridRule>() { changesOnce, changesUnlimited }, new BruteForce(), true);
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(changesOnce.CallCounter == 1);
            Assert.IsTrue(changesUnlimited.CallCounter > 1);
        }

        [TestMethod]
        public void TestNonChanging()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.ShipSettings[1] = 1;

            // 1xSUBMARINE
            //   10
            //   --
            // 0|??
            // 1|??

            var solver = new Solver(null, null, new SingleTrivialChange());

            // A non-changing trial and error rule could lead to an infinite recursion
            // => check if instead correct exception
            Assert.ThrowsException<InvalidOperationException>(() => solver.Solve(game));
        }

        [TestMethod]
        public void TestSeveralSolutions()
        {
            int numRows = 2;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.ColumnTally[0] = 1;
            game.ColumnTally[2] = 1;
            game.ShipSettings[1] = 2;

            // 2xSUBMARINE
            //   101
            //   ---
            // 1|???
            // 1|???
            // => Has 2 solutions

            var solver = new Solver(null, null, new BruteForce(), true);
            Assert.AreEqual(2, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            game.Grid.Rollback();

            solver = new Solver(null, null, new BruteForce(), false);
            Assert.AreEqual(1, solver.Solve(game));

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(BimaruValue.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));
        }
    }
}
