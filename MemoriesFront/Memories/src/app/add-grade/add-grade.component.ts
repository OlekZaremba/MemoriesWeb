import { Component } from '@angular/core';
import { CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-add-grade',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-grade.component.html',
  styleUrl: './add-grade.component.css'
})
export class AddGradeComponent {
  showModal = false;

  selectedStudent = '';
  selectedGrade = '';
  gradeType = '';
  comment = '';

  students = ['Jan Kowalski', 'Anna Nowak', 'Piotr Zieliński']; // tymczasowe
  grades = ['1', '2-', '2', '2+', '3-', '3', '3+', '4-', '4', '4+', '5-', '5', '5+', '6'];

  openModal() {
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  saveGrade() {
    console.log('Zapisano ocenę:', {
      student: this.selectedStudent,
      grade: this.selectedGrade,
      type: this.gradeType,
      comment: this.comment
    });
    this.closeModal();
  }
}
