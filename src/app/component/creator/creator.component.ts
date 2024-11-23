import { Component, getNgModuleById } from '@angular/core';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {FormsModule} from '@angular/forms';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-creator',
  imports: [FormsModule, MatFormFieldModule, MatInputModule, CommonModule],
  templateUrl: './creator.component.html',
  styleUrl: './creator.component.scss'
})
export class CreatorComponent {
  setName: string = 'Brak nazwy'; // Domyślna nazwa zestawu
  questions: string[] = []; // Lista pytań

  addSet() {
    const input = (document.getElementById('setName') as HTMLInputElement).value;
    if (input.trim()) {
      this.setName = input;
      (document.getElementById('setName') as HTMLInputElement).value = '';
    }
  }

  addQuestion() {
    const input = (document.getElementById('question') as HTMLInputElement).value;
    if (input.trim()) {
      this.questions.push(input);
      (document.getElementById('question') as HTMLInputElement).value = '';
    }
  }

  deleteQuestion(index: number) {
    this.questions.splice(index, 1);
  }

  editQuestion(index: number) {
    const newQuestion = prompt('Edytuj pytanie:', this.questions[index]);
    if (newQuestion !== null && newQuestion.trim()) {
      this.questions[index] = newQuestion;
    }
  }
}
