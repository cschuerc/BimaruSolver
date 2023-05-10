using System.Linq;
using Bimaru.Interface.Database;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;
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

        public static T GetObjectResultContent<T>(this ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result)?.Value;
        }

        public static void AssertEqualsOmitGridValues(this GameDto expectedValue, GameDto actualValue)
        {
            Assert.Equal(expectedValue.NumberOfRows, actualValue.NumberOfRows);
            Assert.Equal(expectedValue.NumberOfColumns, actualValue.NumberOfColumns);
            Assert.Equal(expectedValue.TargetNumberOfShipFieldsPerRow, actualValue.TargetNumberOfShipFieldsPerRow);
            Assert.Equal(expectedValue.TargetNumberOfShipFieldsPerColumn, actualValue.TargetNumberOfShipFieldsPerColumn);
            Assert.Equal(expectedValue.TargetNumberOfShipsPerLength, actualValue.TargetNumberOfShipsPerLength);
        }

        public static void AssertEquals(this GameDto expectedValue, GameDto actualValue)
        {
            expectedValue.AssertEqualsOmitGridValues(actualValue);

            foreach (var (expected, actual) in expectedValue.GridValues.Zip(actualValue.GridValues))
            {
                expected.AssertEquals(actual);
            }
        }

        public static void AssertEquals(this GridValueDto expectedValue, GridValueDto actualValue)
        {
            Assert.Equal(expectedValue.RowIndex, actualValue.RowIndex);
            Assert.Equal(expectedValue.ColumnIndex, actualValue.ColumnIndex);
            Assert.Equal(expectedValue.Value, actualValue.Value);
        }

        public static void AssertEquals(this GameEntity expectedValue, GameEntity actualValue)
        {
            Assert.Equal(expectedValue.Id, actualValue.Id);
            Assert.Equal(expectedValue.Size, actualValue.Size);
            Assert.Equal(expectedValue.Difficulty, actualValue.Difficulty);
            Assert.Equal(expectedValue.NumberOfRows, actualValue.NumberOfRows);
            Assert.Equal(expectedValue.NumberOfColumns, actualValue.NumberOfColumns);
            Assert.Equal(expectedValue.TargetNumberOfShipFieldsPerRow, actualValue.TargetNumberOfShipFieldsPerRow);
            Assert.Equal(expectedValue.TargetNumberOfShipFieldsPerColumn, actualValue.TargetNumberOfShipFieldsPerColumn);
            Assert.Equal(expectedValue.TargetNumberOfShipsPerLength, actualValue.TargetNumberOfShipsPerLength);

            foreach (var (expected, actual) in expectedValue.GridValues.Zip(actualValue.GridValues))
            {
                expected.AssertEquals(actual);
            }
        }

        public static void AssertEquals(this GridValueEntity expectedValue, GridValueEntity actualValue)
        {
            Assert.Equal(expectedValue.RowIndex, actualValue.RowIndex);
            Assert.Equal(expectedValue.ColumnIndex, actualValue.ColumnIndex);
            Assert.Equal(expectedValue.Value, actualValue.Value);
        }
    }
}
