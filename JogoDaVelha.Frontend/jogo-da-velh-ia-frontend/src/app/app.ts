import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './api';
import { GameBoardComponent } from './game-board/game-board';
import { GameControlsComponent } from './game-controls/game-controls';
import { DatabaseViewerComponent } from './database-viewer/database-viewer';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, GameBoardComponent, GameControlsComponent, DatabaseViewerComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App implements OnInit {
  gameState: GameState = {
    board: Array(9).fill(null),
    isPlayerTurn: true,
    winner: null,
    gameOver: false
  };
  showDatabase = false;
  databaseStates: DatabaseState[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.startNewGame();
  }

  startNewGame(): void {
    this.apiService.startNewGame().subscribe(state => {
      this.gameState = state;
      this.showDatabase = false;
    });
  }

  onCellClick(position: number): void {
    if (!this.gameState.isPlayerTurn || this.gameState.gameOver || this.gameState.board[position]) {
      return;
    }

    this.apiService.makeMove(position, this.gameState).subscribe(newState => {
      this.gameState = newState;
    });
  }

  viewDatabase(): void {
    this.showDatabase = true;
    this.apiService.getDatabaseStates().subscribe(states => {
      this.databaseStates = states;
    });
  }
}

interface GameState {
  board: (string | null)[];
  isPlayerTurn: boolean;
  winner: string | null;
  gameOver: boolean;
}

interface DatabaseState {
  stateHash: string;
  boardState: string;
  wins: number;
  losses: number;
  draws: number;
  lastUpdated: string;
}