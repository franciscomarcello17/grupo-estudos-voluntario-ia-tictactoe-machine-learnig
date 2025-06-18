import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-game-controls',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-controls.html',
  styleUrls: ['./game-controls.css']
})
export class GameControlsComponent {
  @Output() newGame = new EventEmitter<void>();
  @Output() viewDatabase = new EventEmitter<void>();

  onNewGame(): void {
    this.newGame.emit();
  }

  onViewDatabase(): void {
    this.viewDatabase.emit();
  }
}