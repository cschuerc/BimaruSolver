import { Injectable } from '@angular/core';
import { BimaruValue } from '../bimaru/interfaces/bimaruValue';

@Injectable({
  providedIn: 'root'
})

export class BimaruImagesService {
  getImageFromValue(value: BimaruValue): string {
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
