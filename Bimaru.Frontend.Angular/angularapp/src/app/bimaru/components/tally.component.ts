import { Component, Input } from '@angular/core';

@Component({
  selector: 'bim-tally',
  templateUrl: './tally.component.html',
  styleUrls: ['./tally.component.css']
})
export class TallyComponent {
  @Input() isHorizontal: boolean = true;
  @Input() targetNumbers: number[] = [];
}
