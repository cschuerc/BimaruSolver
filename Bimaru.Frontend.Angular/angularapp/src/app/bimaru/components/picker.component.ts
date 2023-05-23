import { Component, EventEmitter, Output } from '@angular/core';
import { BimaruValue } from '../interfaces/bimaruValue';

@Component({
  selector: 'app-picker',
  templateUrl: './picker.component.html',
  styleUrls: ['./picker.component.css']
})

export class PickerComponent {
  pickerValues: BimaruValue[] = Object.values(BimaruValue) as BimaruValue[];
  pickedValue: string = "";
  @Output() pickedValueChanged: EventEmitter<BimaruValue> = new EventEmitter<BimaruValue>;

  onTileClicked(tileValue: string) {
    this.pickedValueChanged.emit(tileValue as BimaruValue);
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
