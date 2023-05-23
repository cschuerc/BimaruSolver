import { Component, Input } from '@angular/core';

import { GridValueDto } from "../interfaces/gridValueDto"
import { BimaruValue } from '../interfaces/bimaruValue';

@Component({
  selector: 'app-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})

export class GridComponent {
  numberOfRows: number = 6;
  numberOfColumns: number = 6;

  gridValues: BimaruValue[][] = Array.from(
      { length: this.numberOfRows },
      () => Array<BimaruValue>(this.numberOfColumns).fill(BimaruValue.Undetermined)
    );

  @Input() pickedValue: BimaruValue = BimaruValue.Water;

  public loadGrid(numberOfRows: number, numberOfColumns: number, gridValues: GridValueDto[]) {
    this.numberOfRows = numberOfRows;
    this.numberOfColumns = numberOfColumns;

    this.gridValues = Array.from(
      { length: this.numberOfRows },
      () => Array<BimaruValue>(this.numberOfColumns).fill(BimaruValue.Undetermined)
    );

    gridValues.forEach( (gridValue) => {
      this.gridValues[gridValue.RowIndex][gridValue.ColumnIndex] = gridValue.Value;
    });
  }

  public getGridValues(): GridValueDto[] {
    let gridValues: GridValueDto[] = [];

    this.gridValues.forEach((gridRow, rowIndex) => {
      gridRow.forEach((value, columnIndex) => {
        gridValues.push({
          RowIndex: rowIndex,
          ColumnIndex: columnIndex,
          Value: value
        })
      })
    });

    return gridValues;
  }

  onTileClicked(rowIndex: number, columnIndex: number): void {
    this.gridValues[rowIndex][columnIndex] = this.pickedValue;
  }

  onTileRightClicked(rowIndex: number, columnIndex: number): boolean {
    this.gridValues[rowIndex][columnIndex] = BimaruValue.Undetermined;
    return false;
  }

  getImageFromValue(value: BimaruValue) {
    switch (value) {
      case BimaruValue.Undetermined: {
        return "/assets/images/undetermined.png";
      }
      case BimaruValue.Water: {
        return "/assets/images/water.png";
      }
      case BimaruValue.ShipContinuedLeft: {
        return "/assets/images/shipContinuedLeft.png";
      }
      case BimaruValue.ShipContinuedUp: {
        return "/assets/images/shipContinuedUp.png";
      }
      case BimaruValue.ShipContinuedRight: {
        return "/assets/images/shipContinuedRight.png";
      }
      case BimaruValue.ShipContinuedDown: {
        return "/assets/images/shipContinuedDown.png";
      }
      case BimaruValue.ShipMiddle: {
        return "/assets/images/shipMiddle.png";
      }
      case BimaruValue.ShipSingle: {
        return "/assets/images/shipSingle.png";
      }
      case BimaruValue.ShipUndetermined: {
        return "/assets/images/shipUndetermined.png";
      }
    }
  }
}
