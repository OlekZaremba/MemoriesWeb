import { Component } from '@angular/core';
import { NgForOf, NgIf } from "@angular/common";
import { ReactiveFormsModule } from "@angular/forms";

@Component({
  selector: 'app-group-grades',
  standalone: true,
  imports: [NgForOf, NgIf, ReactiveFormsModule],
  templateUrl: './group-grades.component.html',
  styleUrl: './group-grades.component.css'
})
export class GroupGradesComponent {
  showModal = false;

  selectedGrade = {
    student: 'Jan Kowalski',
    grade: '2+',
    type: 'Kartkówka',
    comment: 'Uczeń nieprzygotowany, ale próbował.'
  };

  openModal() {
    // Tu docelowo będziesz przekazywać dane konkretnej oceny
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }
}
