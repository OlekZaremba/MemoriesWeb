import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgIf, NgForOf } from '@angular/common';
import { environment } from '../../environments/environment';

interface SubjectDTO {
  id: number;
  className: string;
  teachers?: ClassTeacherDTO[];
}

interface ClassTeacherDTO {
  teacherId: number;
  teacherName: string;
  groupId: number;
  groupName: string;
}

interface GroupMemberDTO {
  id: number;
  teacherName: string;
  groupName: string;
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
  showAssignTeacherModal = false;
  newSubjectName: string = '';

  subjects: SubjectDTO[] = [];
  groupMembers: GroupMemberDTO[] = [];

  selectedSubjectId: number | null = null;
  selectedGroupMemberId: number | null = null;

  ngOnInit(): void {
    this.loadSubjects();
    this.loadGroupMembers();
  }

  loadSubjects() {
    this.http.get<SubjectDTO[]>(`${environment.apiUrl}/classes`).subscribe({
      next: res => {
        this.subjects = res;
        this.subjects.forEach(subject => {
          this.http.get<ClassTeacherDTO[]>(`${environment.apiUrl}/class/${subject.id}/teachers`)
            .subscribe({
              next: teachers => subject.teachers = teachers,
              error: err => console.error(`Błąd ładowania nauczycieli dla przedmiotu ${subject.className}:`, err)
            });
        });
      },
      error: err => console.error('Błąd ładowania przedmiotów:', err)
    });
  }

  loadGroupMembers() {
    this.http.get<GroupMemberDTO[]>(`${environment.apiUrl}/group-members/teachers-with-groups`).subscribe({
      next: res => this.groupMembers = res,
      error: err => console.error('Błąd ładowania relacji nauczyciel-klasa:', err)
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
    const name = this.newSubjectName.trim();
    if (!name) return;
    this.http.post(`${environment.apiUrl}/classes`, { className: name }).subscribe({
      next: () => {
        this.loadSubjects();
        this.closeAddSubjectModal();
      },
      error: err => console.error('Błąd dodawania przedmiotu:', err)
    });
  }

  openAssignTeacherModal(subjectId: number) {
    this.selectedSubjectId = subjectId;
    this.selectedGroupMemberId = null;
    this.showAssignTeacherModal = true;
  }

  closeAssignTeacherModal() {
    this.showAssignTeacherModal = false;
  }

  assignSubjectToGroupMember() {
    console.log('Zapisz kliknięty');
    console.log('selectedGroupMemberId:', this.selectedGroupMemberId);
    console.log('selectedSubjectId:', this.selectedSubjectId);

    if (!this.selectedGroupMemberId || !this.selectedSubjectId) {
      console.warn('Nie wybrano wszystkich wymaganych pól.');
      return;
    }

    this.http.post(`${environment.apiUrl}/group-members/${this.selectedGroupMemberId}/class/${this.selectedSubjectId}`, {})
      .subscribe({
        next: () => {
          console.log('Przypisano przedmiot do nauczyciela w klasie.');
          this.closeAssignTeacherModal();
          this.loadSubjects();
        },
        error: err => console.error('Błąd przypisania przedmiotu:', err)
      });
  }
}
