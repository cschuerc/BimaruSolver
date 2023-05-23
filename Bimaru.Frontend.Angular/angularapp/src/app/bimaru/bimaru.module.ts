import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MaterialModule } from '../shared/material.module';
import { FormsModule } from '@angular/forms';

import { GridComponent } from './components/grid.component';
import { PickerComponent } from './components/picker.component';
import { TallyComponent } from './components/tally.component';
import { GameComponent } from './components/game.component';
import { ShipTargetComponent } from './components/ship-target.component';

@NgModule({
  declarations: [
    GridComponent,
    PickerComponent,
    TallyComponent,
    GameComponent,
    ShipTargetComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    FormsModule
  ],
  exports: [
    GameComponent
  ]
})
export class BimaruModule { }
