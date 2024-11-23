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
  selector: 'app-set',
  imports: [MatCardModule, MatButtonModule, CommonModule, MatDividerModule, MatIconModule, MatDialogModule],
  templateUrl: './set.component.html',
  styleUrl: './set.component.scss'
})
export class SetComponent {

  cards = [
    { title: 'Zestaw pytań -1' },
    { title: 'Zestaw pytań 0' },
    { title: 'Zestaw pytań 1' },
    { title: 'Zestaw pytań 2' },
    { title: 'Zestaw pytań 3' },
    { title: 'Zestaw pytań 4' },
    { title: 'Zestaw pytań 5' },
    { title: 'Zestaw pytań 6' },
    { title: 'Zestaw pytań 7' },
  ];
  

  readonly dialog = inject(MatDialog);

  openDialog(enterAnimationDuration: string, exitAnimationDuration: string): void {
    this.dialog.open(DialogAnimationsExampleDialogComponent, {
      width: '400px',
      enterAnimationDuration,
      exitAnimationDuration,
  
    });
  }

}
