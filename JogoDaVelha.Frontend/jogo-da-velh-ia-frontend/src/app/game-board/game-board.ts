import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-game-board',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-board.html',
  styleUrls: ['./game-board.css']
})
export class GameBoardComponent {
  @Input() board: (string | null)[] = Array(9).fill(null);
  @Input() gameOver = false;
  @Input() winner: string | null = null;
  @Output() cellClick = new EventEmitter<number>();

  getCellClass(cell: string | null): string {
    if (cell === 'X') return 'x-cell';
    if (cell === 'O') return 'o-cell';
    return 'empty-cell';
  }

  onCellClick(index: number): void {
    this.cellClick.emit(index);
  }
}