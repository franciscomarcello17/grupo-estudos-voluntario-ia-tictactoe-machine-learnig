import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface GameState {
  board: (string | null)[];
  isPlayerTurn: boolean;
  winner: string | null;
  gameOver: boolean;
}

export interface DatabaseState {
  stateHash: string;
  boardState: string;
  wins: number;
  losses: number;
  draws: number;
  lastUpdated: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = '/api/game';

  constructor(private http: HttpClient) { }

  startNewGame(): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/new`, {});
  }

  makeMove(position: number, currentState: GameState): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/move`, {
      position,
      currentState
    });
  }

  getCurrentState(): Observable<GameState> {
    return this.http.get<GameState>(`${this.baseUrl}/current-state`);
  }

  getDatabaseStates(): Observable<DatabaseState[]> {
    return this.http.get<DatabaseState[]>(`${this.baseUrl}/database-states`);
  }
}