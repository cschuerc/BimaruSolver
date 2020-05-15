using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BimaruTest
{
    public static class ExtensionsForTesting
    {
        public static void AssertEqual(this IGameWithMetaInfo expectedGame, IGameWithMetaInfo actualGame)
        {
            if (expectedGame == null && actualGame == null)
            {
                return;
            }

            Assert.IsTrue(expectedGame != null && actualGame != null);

            Assert.AreEqual(expectedGame.MetaInfo, actualGame.MetaInfo);

            expectedGame.Game.AssertEqual(actualGame.Game);
        }

        public static void AssertEqual(this IGame expectedGame, IGame actualGame)
        {
            Assert.IsTrue(expectedGame.TargetNumberOfShipFieldsPerRow.SequenceEqual(actualGame.TargetNumberOfShipFieldsPerRow));
            Assert.IsTrue(expectedGame.TargetNumberOfShipFieldsPerColumn.SequenceEqual(actualGame.TargetNumberOfShipFieldsPerColumn));

            Assert.AreEqual(expectedGame.TargetNumberOfShipsPerLength.LongestShipLength, actualGame.TargetNumberOfShipsPerLength.LongestShipLength);

            if (expectedGame.TargetNumberOfShipsPerLength.LongestShipLength.HasValue)
            {
                foreach (int shipLength in Enumerable.Range(0, expectedGame.TargetNumberOfShipsPerLength.LongestShipLength.Value))
                {
                    Assert.AreEqual(expectedGame.TargetNumberOfShipsPerLength[shipLength], actualGame.TargetNumberOfShipsPerLength[shipLength]);
                }
            }

            expectedGame.Grid.AssertEqual(actualGame.Grid);

            Assert.AreEqual(expectedGame.IsSolved, actualGame.IsSolved);
            Assert.AreEqual(expectedGame.IsUnsolvable, actualGame.IsUnsolvable);
            Assert.AreEqual(expectedGame.IsValid, actualGame.IsValid);
        }

        public static void AssertEqual(this IBimaruGrid expectedGrid, IBimaruGrid actualGrid)
        {
            Assert.AreEqual(expectedGrid.NumberOfRows, actualGrid.NumberOfRows);
            Assert.AreEqual(expectedGrid.NumberOfColumns, actualGrid.NumberOfColumns);

            foreach (var p in expectedGrid.AllPoints())
            {
                Assert.AreEqual(expectedGrid[p], actualGrid[p]);
            }

            Assert.IsTrue(expectedGrid.NumberOfUndeterminedFieldsPerColumn.SequenceEqual(actualGrid.NumberOfUndeterminedFieldsPerColumn));
            Assert.IsTrue(expectedGrid.NumberOfUndeterminedFieldsPerRow.SequenceEqual(actualGrid.NumberOfUndeterminedFieldsPerRow));
            Assert.IsTrue(expectedGrid.NumberOfShipFieldsPerColumn.SequenceEqual(actualGrid.NumberOfShipFieldsPerColumn));
            Assert.IsTrue(expectedGrid.NumberOfShipFieldsPerRow.SequenceEqual(actualGrid.NumberOfShipFieldsPerRow));

            Assert.IsTrue(expectedGrid.NumberOfShipsPerLength.SequenceEqual(actualGrid.NumberOfShipsPerLength));

            Assert.AreEqual(expectedGrid.IsValid, actualGrid.IsValid);
            Assert.AreEqual(expectedGrid.IsFullyDetermined, actualGrid.IsFullyDetermined);
        }

        public static void AssertEqual(this IBimaruGrid actualGrid, BimaruValue[,] expectedFieldValues)
        {
            foreach (var p in actualGrid.AllPoints())
            {
                Assert.AreEqual(expectedFieldValues[p.RowIndex, p.ColumnIndex], actualGrid[p]);
            }
        }
    }
}
