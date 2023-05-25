import { Component, Input } from '@angular/core';
import { BimaruValue } from '../interfaces/bimaruValue';
import { Game } from '../interfaces/game';

@Component({
  selector: 'bim-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent {
  pickedValue: BimaruValue = BimaruValue.Water;
  @Input() game!: Game;

  onPickedValueChanged(pickedValue: BimaruValue) {
    this.pickedValue = pickedValue;
  }
}
