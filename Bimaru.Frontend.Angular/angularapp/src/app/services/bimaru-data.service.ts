import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GameDto } from '../bimaru/interfaces/gameDto';
import { Observable, map, tap } from 'rxjs';
import { Game } from '../bimaru/interfaces/game';
import { GridValueDto } from '../bimaru/interfaces/gridValueDto';
import { BimaruValue } from '../bimaru/interfaces/bimaruValue';
import { GridTile } from '../bimaru/interfaces/gridTile';

@Injectable({
  providedIn: 'root'
})
export class BimaruDataService {
  private webapiBaseUrl = '/api/games';

  constructor(private http: HttpClient) { }

  getRandomBimaruGame(): Observable<Game> {
    return this.http.get<GameDto>(this.webapiBaseUrl).pipe(
      tap(data => console.log('All: ', JSON.stringify(data))),
      map(gameDto => this.convertFromGameDto(gameDto))
    );
  }

  getGameById(id: number): Observable<Game> {
    return this.http.get<GameDto>(`${this.webapiBaseUrl}/${id}`).pipe(
      tap(data => console.log('All: ', JSON.stringify(data))),
      map(gameDto => this.convertFromGameDto(gameDto))
    );
  }

  addGame(game: Game): Observable<Game> {
    return this.http
      .post<GameDto>(this.webapiBaseUrl, this.convertToGameDto(game))
      .pipe(
        map(gameDto => this.convertFromGameDto(gameDto))
      );
  }

  solveGame(game: Game): Observable<Game> {
    return this.http
      .post<GameDto>(`${this.webapiBaseUrl}/solve`, this.convertToGameDto(game))
      .pipe(
        map(gameDto => this.convertFromGameDto(gameDto))
      );
  }

  convertFromGameDto(gameDto: GameDto): Game {
    return {
      NumberOfRows: gameDto.NumberOfRows,
      NumberOfColumns: gameDto.NumberOfColumns,
      TargetNumberOfShipFieldsPerRow: gameDto.TargetNumberOfShipFieldsPerRow,
      TargetNumberOfShipFieldsPerColumn: gameDto.TargetNumberOfShipFieldsPerColumn,
      TargetNumberOfShipsPerLength: gameDto.TargetNumberOfShipsPerLength,
      GridTiles: this.convertFromGridValueDtos(gameDto.GridValues, gameDto.NumberOfRows, gameDto.NumberOfColumns)
    };
  }

  convertFromGridValueDtos(gridValuesDto: GridValueDto[], numberOfRows: number, numberOfColumns: number): GridTile[][] {
    let gridTiles = Array.from(
      { length: numberOfRows },
      () => Array.from({length: numberOfColumns },
        () => <GridTile> {
          value: BimaruValue.Undetermined,
          isReadOnly: false
        })
    );

    gridValuesDto.forEach( (gridValue) => {
      let tile = gridTiles[gridValue.RowIndex][gridValue.ColumnIndex];

      tile.value = gridValue.Value;
      tile.isReadOnly = true;
    });

    return gridTiles;
  }

  convertToGameDto(game: Game): GameDto {
    return {
      NumberOfRows: game.NumberOfRows,
      NumberOfColumns: game.NumberOfColumns,
      TargetNumberOfShipFieldsPerRow: game.TargetNumberOfShipFieldsPerRow,
      TargetNumberOfShipFieldsPerColumn: game.TargetNumberOfShipFieldsPerColumn,
      TargetNumberOfShipsPerLength: game.TargetNumberOfShipsPerLength,
      GridValues: this.convertToGridValueDtos(game.GridTiles)
    }
  }

  convertToGridValueDtos(gridTiles: GridTile[][]): GridValueDto[] {
    let gridValueDtos: GridValueDto[] = [];

    gridTiles.forEach((tilesRow, rowIndex) => {
      tilesRow.forEach((tile, columnIndex) => {
        gridValueDtos.push({
          RowIndex: rowIndex,
          ColumnIndex: columnIndex,
          Value: tile.value
        })
      })
    });

    return gridValueDtos;
  }
}
