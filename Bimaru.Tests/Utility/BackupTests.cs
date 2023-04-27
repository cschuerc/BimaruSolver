using System;
using Bimaru.Game;
using Bimaru.Interface;
using Bimaru.Interface.Game;
using Bimaru.Utility;
using Xunit;

namespace Bimaru.Tests.Utility
{
    public class BackupTests
    {
        [Fact]
        public void TestSetSavePointNull()
        {
            var backup = new Backup<BimaruGrid>();

            Assert.Throws<ArgumentNullException>(() => backup.SetSavePoint(null));
        }

        [Fact]
        public void TestRestoreSavePointToNull()
        {
            var backup = new Backup<BimaruGrid>();

            Assert.Throws<ArgumentNullException>(() => backup.RestoreAndDeleteLastSavepoint(null));
        }

        [Fact]
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

            var expectedFieldValues = new[,]
            {
                { BimaruValue.SHIP_SINGLE, BimaruValue.WATER }
            };

            grid.AssertEqual(expectedFieldValues);

            AssertEmptyStack(backup);
        }

        private static void AssertEmptyStack(IBackup<BimaruGrid> backup)
        {
            var destination = new BimaruGrid(1, 1);
            Assert.Throws<InvalidOperationException>(() => backup.RestoreAndDeleteLastSavepoint(destination));
        }

        [Fact]
        public void TestCloneNullToClipboard()
        {
            var backup = new Backup<BimaruGrid>();

            Assert.Throws<ArgumentNullException>(() => backup.CloneToClipboard(null));
        }

        [Fact]
        public void TestRestoreClipBoardToNull()
        {
            var backup = new Backup<BimaruGrid>();
            backup.CloneToClipboard(new BimaruGrid(1, 1));

            Assert.Throws<ArgumentNullException>(() => backup.RestoreFromClipboardTo(null));
        }

        [Fact]
        public void TestRestoreEmptyClipBoard()
        {
            var backup = new Backup<BimaruGrid>();
            var grid = new BimaruGrid(1, 1);

            Assert.Throws<InvalidOperationException>(() => backup.RestoreFromClipboardTo(grid));
        }

        [Fact]
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

            var expectedFieldValues = new[,]
            {
                { BimaruValue.SHIP_CONT_UP, BimaruValue.SHIP_CONT_DOWN }
            };

            grid.AssertEqual(expectedFieldValues);
        }
    }
}
