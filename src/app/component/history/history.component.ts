import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {MatCardModule} from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DialogAnimationsExampleDialogComponent } from '../dialog-animations-example-dialog/dialog-animations-example-dialog.component';
import { Inject } from '@angular/core';

@Component({
  selector: 'app-history',
  imports: [MatCardModule, MatButtonModule, CommonModule, MatDividerModule, MatIconModule, MatDialogModule],
  templateUrl: './history.component.html',
  styleUrl: './history.component.scss'
})
export class HistoryComponent {

  cards = [
    { title: 'Zestaw 1', date: '2024-11-21' },
    { title: 'Zestaw 2', date: '2023-12-25' },
    { title: 'Zestaw 1', date: '2024-1-21' },
    { title: 'Zestaw 2', date: '2023-5-25' },
    { title: 'Zestaw 2', date: '2023-6-25' },
    { title: 'Zestaw 2', date: '2023-2-25' },
    { title: 'Zestaw 2', date: '2023-12-25' },
    { title: 'Zestaw 2', date: '2023-8-25' },
  ];

  sortCards(criteria: 'title' | 'date') {
    this.cards.sort((a, b) => {
      if (criteria === 'title') {
        return a.title.localeCompare(b.title);
      } else if (criteria === 'date') {
        return new Date(b.date).getTime() - new Date(a.date).getTime(); // Sort malejąco po dacie
      }
      return 0;
    });
  }

  readonly dialog = inject(MatDialog);

  openDialog(enterAnimationDuration: string, exitAnimationDuration: string): void {
    this.dialog.open(DialogAnimationsExampleDialogComponent, {
      width: '400px',
      enterAnimationDuration,
      exitAnimationDuration,
  
    });
  }
}
