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

interface GroupDTO {
  id: number;
  groupName: string;
}

interface TeacherDTO {
  id: number;
  name: string;
  surname: string;
}

interface ClassTeacherDTO {
  teacherId: number;
  teacherName: string;
  groupId: number;
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
  groups: GroupDTO[] = [];
  teachers: TeacherDTO[] = [];

  selectedGroupId: number | null = null;
  selectedSubjectId: number | null = null;
  selectedTeacherId: number | null = null;

  ngOnInit(): void {
    this.loadSubjects();
    this.loadGroups();
    this.loadTeachers();
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

  loadGroups() {
    this.http.get<GroupDTO[]>(`${environment.apiUrl}/groups`).subscribe({
      next: res => this.groups = res,
      error: err => console.error('Błąd ładowania grup:', err)
    });
  }

  loadTeachers() {
    this.http.get<TeacherDTO[]>(`${environment.apiUrl}/users/teachers`).subscribe({
      next: res => this.teachers = res,
      error: err => console.error('Błąd ładowania nauczycieli:', err)
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
    this.selectedTeacherId = null;
    this.selectedGroupId = null;
    this.showAssignTeacherModal = true;
  }

  closeAssignTeacherModal() {
    this.showAssignTeacherModal = false;
  }

  assignTeacherToGroupAndSubject() {
    if (!this.selectedTeacherId || !this.selectedGroupId || !this.selectedSubjectId) return;

    this.http.post(`${environment.apiUrl}/assignments/teacher/${this.selectedTeacherId}/group/${this.selectedGroupId}`, {})
      .subscribe({
        next: () => {
          this.getGroupMemberId(this.selectedTeacherId!, this.selectedGroupId!).then(groupMemberId => {
            console.log('Resolved groupMemberId:', groupMemberId);
            if (!groupMemberId) {
              console.error('Nie znaleziono przypisanego nauczyciela do klasy.');
              return;
            }

            this.http.post(`${environment.apiUrl}/group-members/${groupMemberId}/class/${this.selectedSubjectId}`, {})
              .subscribe({
                next: () => {
                  console.log('Pełna relacja nauczyciel-klasa-przedmiot zapisana.');
                  this.closeAssignTeacherModal();
                  this.loadSubjects();
                },
                error: err => {
                  console.error('Błąd przypisania przedmiotu:', err);
                }
              });
          });
        },
        error: err => {
          if (err.status === 400) {
            console.warn('Nauczyciel już przypisany do klasy – kontynuuję...');
            this.getGroupMemberId(this.selectedTeacherId!, this.selectedGroupId!).then(groupMemberId => {
              console.log('Resolved groupMemberId (fallback):', groupMemberId);
              if (!groupMemberId) return;

              this.http.post(`${environment.apiUrl}/group-members/${groupMemberId}/class/${this.selectedSubjectId}`, {})
                .subscribe({
                  next: () => {
                    console.log('Przypisano przedmiot mimo wcześniejszej relacji.');
                    this.closeAssignTeacherModal();
                    this.loadSubjects();
                  },
                  error: err => {
                    console.error('Błąd przypisania przedmiotu:', err);
                  }
                });
            });
          } else {
            console.error('Błąd przypisywania nauczyciela do klasy:', err);
          }
        }
      });
  }

  private getGroupMemberId(teacherId: number, groupId: number): Promise<number | null> {
    return new Promise((resolve) => {
      this.http.get<{ id: number } | null>(
        `${environment.apiUrl}/group-members/teacher/${teacherId}/group/${groupId}`
      ).subscribe({
        next: res => resolve(res?.id ?? null),
        error: err => {
          console.error('Błąd pobierania groupMemberId:', err);
          resolve(null);
        }
      });
    });
  }
}
