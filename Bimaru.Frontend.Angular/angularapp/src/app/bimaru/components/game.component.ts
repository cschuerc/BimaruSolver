import { Component, ViewChild } from '@angular/core';
import { BimaruValue } from '../interfaces/bimaruValue';
import { GameDto } from '../interfaces/gameDto';
import { TallyComponent } from './tally.component';
import { GridComponent } from './grid.component';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent {
  pickedValue: BimaruValue = BimaruValue.Water;
  @ViewChild('rowTally') rowTally!: TallyComponent;
  @ViewChild('columnTally') columnTally!: TallyComponent;
  @ViewChild(GridComponent) grid!: GridComponent;

  onPickedValueChanged(pickedValue: BimaruValue) {
    this.pickedValue = pickedValue;
  }

  loadGame(game: GameDto) {
    this.rowTally.loadTally(game.TargetNumberOfShipFieldsPerRow);
    this.columnTally.loadTally(game.TargetNumberOfShipFieldsPerColumn);
    this.grid.loadGrid(game.NumberOfRows, game.NumberOfColumns, game.GridValues);
  }

  getGame(): GameDto {
    return <GameDto>{
      NumberOfRows: this.grid.numberOfRows,
      NumberOfColumns: this.grid.numberOfColumns,
      TargetNumberOfShipFieldsPerRow: this.rowTally.targetNumbers,
      TargetNumberOfShipFieldsPerColumn: this.columnTally.targetNumbers,
      TargetNumberOfShipsPerLength: [4, 3, 2, 1],
      GridValues: this.grid.getGridValues()
    }
  }
}
