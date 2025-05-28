import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgIf, NgForOf } from '@angular/common';
import { environment } from '../../environments/environment';

interface SubjectDTO {
  id: number;
  className: string;
}

@Component({
  selector: 'app-classes',
  standalone: true,
  imports: [NgIf, FormsModule, NgForOf],
  templateUrl: './classes.component.html',
  styleUrl: './classes.component.css'
})
export class ClassesComponent implements OnInit {
  constructor(private http: HttpClient) {}

  showAddSubjectModal = false;
  newSubjectName: string = '';
  showAssignTeacherModal = false;
  availableTeachers = ['Jan Kowalski', 'Anna Nowak', 'Tomasz Zięba'];
  selectedTeachers: string[] = [];

  subjects: SubjectDTO[] = [];

  ngOnInit(): void {
    this.loadSubjects();
  }

  loadSubjects(): void {
    this.http.get<SubjectDTO[]>(`${environment.apiUrl}/classes`).subscribe({
      next: (res) => this.subjects = res,
      error: (err) => console.error('Błąd przy pobieraniu przedmiotów:', err)
    });
  }

  openAddSubjectModal() {
    this.newSubjectName = '';
    this.showAddSubjectModal = true;
  }

  closeAddSubjectModal() {
    this.showAddSubjectModal = false;
  }

  addSubject() {
    const trimmedName = this.newSubjectName.trim();
    if (!trimmedName) return;

    const body = {
      className: trimmedName
    };

    this.http.post(`${environment.apiUrl}/classes`, body).subscribe({
      next: () => {
        console.log('Przedmiot dodany:', trimmedName);
        this.loadSubjects();
        this.closeAddSubjectModal();
      },
      error: (err) => {
        console.error('Błąd przy dodawaniu przedmiotu:', err);
      }
    });
  }

  openAssignTeacherModal() {
    this.selectedTeachers = [];
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
    console.log('Przypisano nauczycieli:', this.selectedTeachers);
    this.closeAssignTeacherModal();
  }
}
