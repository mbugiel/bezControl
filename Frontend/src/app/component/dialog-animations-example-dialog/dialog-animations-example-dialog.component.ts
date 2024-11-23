import { Component } from '@angular/core';

@Component({
  selector: 'app-dialog-animations-example-dialog',
  imports: [],
  templateUrl: './dialog-animations-example-dialog.component.html',
  styleUrl: './dialog-animations-example-dialog.component.scss'
})
export class DialogAnimationsExampleDialogComponent {
  dialogOpen: boolean = false; // Zmienna do kontrolowania widoczności dialogu

  openDialog() {
    this.dialogOpen = true; // Otwórz dialog
  }

  closeDialog() {
    this.dialogOpen = false; // Zamknij dialog
  }

  confirm() {
    alert('Potwierdzono!');
    this.closeDialog(); // Zamknij dialog po potwierdzeniu
  }
}
