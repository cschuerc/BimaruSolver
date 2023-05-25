import { Component, Input } from '@angular/core';

import { GridValueDto } from "../interfaces/gridValueDto"
import { BimaruValue } from '../interfaces/bimaruValue';
import { BimaruImagesService } from 'src/app/services/bimaru-images.service';

@Component({
  selector: 'bim-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})

export class GridComponent {
  @Input() numberOfRows!: number;
  @Input() numberOfColumns!: number;
  @Input() gridValues!: BimaruValue[][];
  @Input() pickedValue: BimaruValue = BimaruValue.Water;

  constructor(private imageService: BimaruImagesService) {}

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
