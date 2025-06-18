import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatabaseState } from '../api';

@Component({
  selector: 'app-database-viewer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './database-viewer.html',
  styleUrls: ['./database-viewer.css']
})
export class DatabaseViewerComponent {
  @Input() databaseStates: DatabaseState[] = [];
}