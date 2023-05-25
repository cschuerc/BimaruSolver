import { Component, Input } from '@angular/core';

import { BimaruValue } from '../interfaces/bimaruValue';
import { BimaruImagesService } from 'src/app/services/bimaru-images.service';
import { GridTile } from '../interfaces/gridTile';

@Component({
  selector: 'bim-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})

export class GridComponent {
  @Input() numberOfRows!: number;
  @Input() numberOfColumns!: number;
  @Input() tiles!: GridTile[][];
  @Input() pickedValue: BimaruValue = BimaruValue.Water;

  constructor(private imageService: BimaruImagesService) {}

  getImageFromTile(tile: GridTile): string {
    return this.imageService.getImageFromValue(tile.value)
  }

  onTileClicked(tile: GridTile): void {
    this.setTileValue(tile, this.pickedValue);
  }

  onTileRightClicked(tile: GridTile): boolean {
    this.setTileValue(tile, BimaruValue.Undetermined);
    
    return false;
  }

  private setTileValue(tile: GridTile, value: BimaruValue) {
    if (!tile.isReadOnly) {
      tile.value = value;
    }
  }
}
