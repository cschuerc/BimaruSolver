using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Utility
{
    [TestClass]
    public class BackupTests
    {
        [TestMethod]
        public void TestSetSavePointNull()
        {
            var stack = new Backup<BimaruGrid>();

            Assert.ThrowsException<ArgumentNullException>(() => stack.SetSavePoint(null));
        }

        [TestMethod]
        public void TestRestoreSavePointToNull()
        {
            var backup = new Backup<BimaruGrid>();

            Assert.ThrowsException<ArgumentNullException>(() => backup.RestoreAndDeleteLastSavepoint(null));
        }

        [TestMethod]
        public void TestStackOperations()
        {
            var backup = new Backup<BimaruGrid>();

            var grid = new BimaruGrid(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            grid[p0] = BimaruValue.SHIP_SINGLE;
            grid[p1] = BimaruValue.WATER;

            backup.SetSavePoint(grid);

            grid[p0] = BimaruValue.SHIP_CONT_DOWN;
            grid[p1] = BimaruValue.UNDETERMINED;

            backup.RestoreAndDeleteLastSavepoint(grid);

            BimaruValue[,] expectedFieldValues = new BimaruValue[1, 2]
            {
                { BimaruValue.SHIP_SINGLE, BimaruValue.WATER }
            };

            grid.AssertEqual(expectedFieldValues);

            AssertEmptyStack(backup);
        }

        private void AssertEmptyStack(Backup<BimaruGrid> backup)
        {
            var destination = new BimaruGrid(1, 1);
            Assert.ThrowsException<InvalidOperationException>(() => backup.RestoreAndDeleteLastSavepoint(destination));
        }

        [TestMethod]
        public void TestCloneNullToClipboard()
        {
            var backup = new Backup<BimaruGrid>();

            Assert.ThrowsException<ArgumentNullException>(() => backup.CloneToClipboard(null));
        }

        [TestMethod]
        public void TestRestoreClipBoardToNull()
        {
            var backup = new Backup<BimaruGrid>();
            backup.CloneToClipboard(new BimaruGrid(1, 1));

            Assert.ThrowsException<ArgumentNullException>(() => backup.RestoreFromClipboardTo(null));
        }

        [TestMethod]
        public void TestRestoreEmptyClipBoard()
        {
            var backup = new Backup<BimaruGrid>();
            var grid = new BimaruGrid(1, 1);

            Assert.ThrowsException<InvalidOperationException>(() => backup.RestoreFromClipboardTo(grid));
        }

        [TestMethod]
        public void TestClipboard()
        {
            var backup = new Backup<BimaruGrid>();

            var grid = new BimaruGrid(1, 2);
            var p0 = new GridPoint(0, 0);
            var p1 = new GridPoint(0, 1);

            grid[p0] = BimaruValue.SHIP_CONT_UP;
            grid[p1] = BimaruValue.SHIP_CONT_DOWN;

            backup.CloneToClipboard(grid);

            grid[p0] = BimaruValue.SHIP_SINGLE;
            grid[p1] = BimaruValue.WATER;

            backup.RestoreFromClipboardTo(grid);

            BimaruValue[,] expectedFieldValues;

            expectedFieldValues = new BimaruValue[1, 2]
            {
                { BimaruValue.SHIP_CONT_UP, BimaruValue.SHIP_CONT_DOWN }
            };

            grid.AssertEqual(expectedFieldValues);
        }
    }
}
