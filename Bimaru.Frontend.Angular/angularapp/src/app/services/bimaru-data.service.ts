import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GameDto } from '../bimaru/interfaces/gameDto';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BimaruDataService {
  private webapiBaseUrl = '/api/games';

  constructor(private http: HttpClient) { }

  getRandomBimaruGame(): Observable<GameDto> {
    return this.http.get<GameDto>(this.webapiBaseUrl).pipe(
      tap(data => console.log('All: ', JSON.stringify(data)))
    );
  }

  getGameById(id: number): Observable<GameDto> {
    return this.http.get<GameDto>(`${this.webapiBaseUrl}/${id}`).pipe(
      tap(data => console.log('All: ', JSON.stringify(data)))
    );
  }

  addGame(game: GameDto): Observable<GameDto> {
    return this.http.post<GameDto>(this.webapiBaseUrl, game);
  }

  solveGame(game: GameDto): Observable<GameDto> {
    return this.http.post<GameDto>(`${this.webapiBaseUrl}/solve`, game);
  }
}
