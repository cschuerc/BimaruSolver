import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { BimaruDataService } from './services/bimaru-data.service';
import { Game } from './bimaru/interfaces/game';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements AfterViewInit, OnInit {
  game!: Game;
  sub!: Subscription;

  constructor(
    private bimaruDataService: BimaruDataService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadNewGame();
  }

  ngAfterViewInit(): void {
    this.loadNewGame();
    this.changeDetector.detectChanges();
  }

  loadNewGame(): Subscription {
    return this.bimaruDataService
      .getRandomBimaruGame()
      .subscribe({
        next: game => this.game = game,
        error: err => console.error(`An error occurred: ${err.status}, error message is: ${err.message}`)
      });
  }

  solveGame(): Subscription {
    return this.bimaruDataService
      .solveGame(this.game)
      .subscribe({
        next: game => this.game = game,
        error: err => console.error(`An error occurred: ${err.status}, error message is: ${err.message}`)
      });
  }
}