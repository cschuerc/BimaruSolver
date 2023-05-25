import { Component, EventEmitter, Output } from '@angular/core';
import { BimaruValue } from '../interfaces/bimaruValue';
import { BimaruImagesService } from 'src/app/services/bimaru-images.service';

@Component({
  selector: 'bim-picker',
  templateUrl: './picker.component.html',
  styleUrls: ['./picker.component.css']
})

export class PickerComponent {
  pickerValues: BimaruValue[] = Object.values(BimaruValue) as BimaruValue[];
  pickedValue: string = "";
  @Output() pickedValueChanged: EventEmitter<BimaruValue> = new EventEmitter<BimaruValue>;

  constructor(private imageService: BimaruImagesService) {}

  getImageFromValue(value: BimaruValue): string {
    return this.imageService.getImageFromValue(value);
  }

  onTileClicked(tileValue: string) {
    this.pickedValueChanged.emit(tileValue as BimaruValue);
  }
}
