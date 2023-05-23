import { Component, Input } from '@angular/core';

import { GridValueDto } from "../interfaces/gridValueDto"
import { BimaruValue } from '../interfaces/bimaruValue';
import { BimaruImagesService } from 'src/app/services/bimaru-images.service';

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

  constructor(private imageService: BimaruImagesService) {

  }

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

  getImageFromIndices(rowIndex: number, columnIndex: number): string {
    return this.imageService.getImageFromValue(this.gridValues[rowIndex][columnIndex])
  }

  onTileClicked(rowIndex: number, columnIndex: number): void {
    this.gridValues[rowIndex][columnIndex] = this.pickedValue;
  }

  onTileRightClicked(rowIndex: number, columnIndex: number): boolean {
    this.gridValues[rowIndex][columnIndex] = BimaruValue.Undetermined;
    return false;
  }
}
