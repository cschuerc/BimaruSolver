import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { GameComponent } from './bimaru/components/game.component';
import { Subscription } from 'rxjs';
import { BimaruDataService } from './services/bimaru-data.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements AfterViewInit, OnInit, OnDestroy {
  @ViewChild(GameComponent) game!: GameComponent;
  sub!: Subscription;

  constructor(
    private bimaruDataService: BimaruDataService,
    private changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.sub = this.loadNewGame();
  }

  ngAfterViewInit(): void {
    this.loadNewGame();
    this.changeDetector.detectChanges();
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  loadNewGame(): Subscription {
    return this.bimaruDataService
      .getRandomBimaruGame()
      .subscribe({
        next: gameDto => this.game.loadGame(gameDto),
        error: err => console.error(`An error occurred: ${err.status}, error message is: ${err.message}`)
      });
  }

  solveGame(): Subscription {
    return this.bimaruDataService
      .solveGame(this.game.getGame())
      .subscribe({
        next: gameDto => this.game.loadGame(gameDto),
        error: err => console.error(`An error occurred: ${err.status}, error message is: ${err.message}`)
      });
  }
}