using BimaruGame;
using BimaruInterfaces;
using BimaruSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Utility;

namespace BimaruTest
{
    [TestClass]
    public class SolverTests
    {
        private static Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            Grid initialGrid = new Grid(numRows, numColumns);
            RollbackGrid grid = new RollbackGrid(initialGrid);

            return new Game(rowTally, columnTally, settings, grid);
        }

        [TestMethod]
        public void TestNullArguments()
        {
            int numRows = 4;
            int numColumns = 3;

            var game = SetupGame(numRows, numColumns);

            var emptyChangedRules = new List<IFieldChangedRule>();
            var emptyFullGridRules = new List<IFullGridRule>();
            var solver = new Solver(null, null, null);
        }

        [TestMethod]
        public void TestInvalidGame()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;

            var solver = new Solver(null, null, null);

            // Sum(RowTally) != Sum(ColumnTally) => Invalid
            Assert.ThrowsException<InvalidBimaruGame>(() => solver.Solve(game));
        }

        [TestMethod]
        public void TestTrialAndErrorRuleSuccess()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            var solver = new Solver(null, null, new BruteForce());
            solver.Solve(game);

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.RowTally[1] = 1;
            game.ColumnTally[1] = 2;
            game.Settings[2] = 1;

            solver.Solve(game);
            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.SHIP_CONT_UP, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.SHIP_CONT_DOWN, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            game.Grid.Rollback();
            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));
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
            game.Settings[1] = 2;

            var solver = new Solver(null, null, new BruteForce());
            solver.Solve(game);

            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            // No solution => Only a single grid on the stack
            Assert.ThrowsException<InvalidOperationException>(() => game.Grid.Rollback());
        }

        private class ChangeLogger : IFieldChangedRule
        {
            public IList<FieldValueChangedEventArgs<FieldValues>> Changes
            {
                get;
                private set;
            }

            public ChangeLogger()
            {
                Changes = new List<FieldValueChangedEventArgs<FieldValues>>();
            }

            public void FieldValueChanged(IGame game, FieldValueChangedEventArgs<FieldValues> e)
            {
                Changes.Add(e);
            }
        }

        [TestMethod]
        public void TestInitialConditions()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);
            game.Grid.SetFieldValue(new GridPoint(0, 1), FieldValues.WATER);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.Settings[1] = 1;

            var changes = new ChangeLogger();
            var solver = new Solver(new List<IFieldChangedRule>() { changes }, null, null);
            solver.Solve(game);

            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(FieldValues.SHIP_SINGLE, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            Assert.AreEqual(2, changes.Changes.Count);

            Assert.AreEqual(FieldValues.UNDETERMINED, changes.Changes[0].OriginalValue);
            Assert.AreEqual(FieldValues.UNDETERMINED, changes.Changes[1].OriginalValue);

            Assert.AreEqual(new GridPoint(0, 0), changes.Changes[0].Point);
            Assert.AreEqual(new GridPoint(0, 1), changes.Changes[1].Point);
        }

        [TestMethod]
        public void TestCorrectUnsubscription()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.Settings[1] = 1;

            var changes = new ChangeLogger();
            var solver = new Solver(new List<IFieldChangedRule>() { changes }, null, null);
            solver.Solve(game);

            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_SINGLE);

            Assert.AreEqual(0, changes.Changes.Count);

            Assert.IsFalse(game.IsSolved);
            Assert.AreEqual(FieldValues.SHIP_SINGLE, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.UNDETERMINED, game.Grid.GetFieldValue(new GridPoint(1, 1)));

            // No InvalidFieldChange Exception here
            game.Grid.SetFieldValue(new GridPoint(0, 0), FieldValues.SHIP_CONT_LEFT);
        }

        [TestMethod]
        public void TestFullGridAndChangedRules()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.Settings[1] = 1;

            var fieldChangedRules = new List<IFieldChangedRule>() { new WaterAroundSingleShip() };
            var fullGridRules = new List<IFullGridRule>() { new OneShipRowCol() };
            var solver = new Solver(fieldChangedRules, fullGridRules, null);
            solver.Solve(game);

            Assert.IsTrue(game.IsSolved);
            Assert.AreEqual(FieldValues.SHIP_SINGLE, game.Grid.GetFieldValue(new GridPoint(0, 0)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(1, 0)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(0, 1)));
            Assert.AreEqual(FieldValues.WATER, game.Grid.GetFieldValue(new GridPoint(1, 1)));
        }

        [TestMethod]
        public void TestNonChanging()
        {
            int numRows = 2;
            int numColumns = 2;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 1;
            game.ColumnTally[0] = 1;
            game.Settings[1] = 1;

            var solver = new Solver(null, null, new MockNonChanging());

            // A non-changing trial and error rule could lead to an infinite recursion
            // => check if instead correct exception
            Assert.ThrowsException<InvalidOperationException>(() => solver.Solve(game));
        }
    }
}
