import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIf, NgForOf } from '@angular/common'

@Component({
  selector: 'app-classes',
  standalone: true,
  imports: [NgIf, FormsModule, NgForOf],
  templateUrl: './classes.component.html',
  styleUrl: './classes.component.css'
})
export class ClassesComponent {
  showAddSubjectModal = false;
  newSubjectName: string = '';
  showAssignTeacherModal = false;
  availableTeachers = ['Jan Kowalski', 'Anna Nowak', 'Tomasz Zięba'];
  selectedTeachers: string[] = [];

  openAddSubjectModal() {
    this.newSubjectName = '';
    this.showAddSubjectModal = true;
  }

  closeAddSubjectModal() {
    this.showAddSubjectModal = false;
  }

  addSubject() {
    if (this.newSubjectName.trim()) {
      console.log('📚 Dodano przedmiot:', this.newSubjectName);
      // tutaj w przyszłości wywołanie do backendu
    }
    this.closeAddSubjectModal();
  }

  openAssignTeacherModal() {
    this.selectedTeachers = []; // reset
    this.showAssignTeacherModal = true;
  }

  closeAssignTeacherModal() {
    this.showAssignTeacherModal = false;
  }

  toggleTeacher(teacher: string, checked: boolean) {
    if (checked) {
      this.selectedTeachers.push(teacher);
    } else {
      this.selectedTeachers = this.selectedTeachers.filter(t => t !== teacher);
    }
  }

  assignTeachers() {
    console.log('📌 Przypisano nauczycieli:', this.selectedTeachers);
    this.closeAssignTeacherModal();
  }
}
