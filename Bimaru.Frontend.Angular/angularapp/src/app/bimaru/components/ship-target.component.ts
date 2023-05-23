import { Component, Input } from '@angular/core';
import { BimaruImagesService } from 'src/app/services/bimaru-images.service';
import { BimaruValue } from '../interfaces/bimaruValue';

@Component({
  selector: 'app-ship-target',
  templateUrl: './ship-target.component.html',
  styleUrls: ['./ship-target.component.css']
})
export class ShipTargetComponent {
  @Input() targetNumberOfShipsPerLength: number[] = [];

  constructor(private imageService: BimaruImagesService) {}

  getImageFromValue(value: BimaruValue): string {
    return this.imageService.getImageFromValue(value);
  }

  getShipValues(length: number): BimaruValue[] {
    let shipValues: BimaruValue[] = [];

    Array.from(Array(this.targetNumberOfShipsPerLength.length - length)).forEach(() => shipValues.push(BimaruValue.Undetermined));

    switch (length) {
      case 0:
        break;
      case 1:
        shipValues.push(BimaruValue.ShipSingle);
        break;
      default:
        shipValues.push(BimaruValue.ShipContinuedRight);
        Array.from(Array(length - 2)).forEach(() => shipValues.push(BimaruValue.ShipMiddle));
        shipValues.push(BimaruValue.ShipContinuedLeft);
        break;
    }

    return shipValues;
  }
}
