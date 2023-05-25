import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GameDto } from '../bimaru/interfaces/gameDto';
import { Observable, map, tap } from 'rxjs';
import { Game } from '../bimaru/interfaces/game';
import { GridValueDto } from '../bimaru/interfaces/gridValueDto';
import { BimaruValue } from '../bimaru/interfaces/bimaruValue';

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
      GridValues: this.convertFromGridValueDtos(gameDto.GridValues, gameDto.NumberOfRows, gameDto.NumberOfColumns)
    };
  }

  convertFromGridValueDtos(gridValuesDto: GridValueDto[], numberOfRows: number, numberOfColumns: number): BimaruValue[][] {
    let gridValues = Array.from(
      { length: numberOfRows },
      () => Array<BimaruValue>(numberOfColumns).fill(BimaruValue.Undetermined)
    );

    gridValuesDto.forEach( (gridValue) => {
      gridValues[gridValue.RowIndex][gridValue.ColumnIndex] = gridValue.Value;
    });

    return gridValues;
  }

  convertToGameDto(game: Game): GameDto {
    return {
      NumberOfRows: game.NumberOfRows,
      NumberOfColumns: game.NumberOfColumns,
      TargetNumberOfShipFieldsPerRow: game.TargetNumberOfShipFieldsPerRow,
      TargetNumberOfShipFieldsPerColumn: game.TargetNumberOfShipFieldsPerColumn,
      TargetNumberOfShipsPerLength: game.TargetNumberOfShipsPerLength,
      GridValues: this.convertToGridValueDtos(game.GridValues)
    }
  }

  convertToGridValueDtos(gridValues: BimaruValue[][]): GridValueDto[] {
    let gridValueDtos: GridValueDto[] = [];

    gridValues.forEach((gridRow, rowIndex) => {
      gridRow.forEach((value, columnIndex) => {
        gridValueDtos.push({
          RowIndex: rowIndex,
          ColumnIndex: columnIndex,
          Value: value
        })
      })
    });

    return gridValueDtos;
  }
}
