using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests
{
    public static class ExtensionsForTesting
    {
        public static void AssertEqual(this IBimaruGrid expectedGrid, IBimaruGrid actualGrid)
        {
            Assert.Equal(expectedGrid.NumberOfRows, actualGrid.NumberOfRows);
            Assert.Equal(expectedGrid.NumberOfColumns, actualGrid.NumberOfColumns);

            foreach (var p in expectedGrid.AllPoints())
            {
                Assert.Equal(expectedGrid[p], actualGrid[p]);
            }

            Assert.True(expectedGrid.NumberOfUndeterminedFieldsPerColumn.SequenceEqual(actualGrid.NumberOfUndeterminedFieldsPerColumn));
            Assert.True(expectedGrid.NumberOfUndeterminedFieldsPerRow.SequenceEqual(actualGrid.NumberOfUndeterminedFieldsPerRow));
            Assert.True(expectedGrid.NumberOfShipFieldsPerColumn.SequenceEqual(actualGrid.NumberOfShipFieldsPerColumn));
            Assert.True(expectedGrid.NumberOfShipFieldsPerRow.SequenceEqual(actualGrid.NumberOfShipFieldsPerRow));

            Assert.True(expectedGrid.NumberOfShipsPerLength.SequenceEqual(actualGrid.NumberOfShipsPerLength));

            Assert.Equal(expectedGrid.IsValid, actualGrid.IsValid);
            Assert.Equal(expectedGrid.IsFullyDetermined, actualGrid.IsFullyDetermined);
        }

        public static void AssertEqual(this IBimaruGrid actualGrid, BimaruValue[,] expectedFieldValues)
        {
            foreach (var p in actualGrid.AllPoints())
            {
                Assert.Equal(expectedFieldValues[p.RowIndex, p.ColumnIndex], actualGrid[p]);
            }
        }
    }
}
