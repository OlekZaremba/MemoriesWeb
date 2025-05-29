import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CommonModule } from '@angular/common';
import { GradeViewComponent } from '../grade-view/grade-view.component';

interface GroupDTO {
  id: number;
  groupName: string;
}

interface SubjectDTO {
  id: number;
  className: string;
  average: number;
}

@Component({
  selector: 'app-grades',
  standalone: true,
  imports: [CommonModule, GradeViewComponent],
  templateUrl: './grades.component.html',
  styleUrl: './grades.component.css'
})
export class GradesComponent implements OnInit {
  @Output() navigateTo = new EventEmitter<string | { view: string, groupId?: number }>();
  userRole: string | null = null;
  teacherGroups: GroupDTO[] = [];
  studentSubjects: SubjectDTO[] = [];
  activeSubjectId: number | null = null;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.userRole = sessionStorage.getItem('userRole');

    if (this.userRole === 'T') {
      this.loadTeacherGroups();
    }

    if (this.userRole === 'S') {
      this.loadStudentSubjects();
    }
  }

  loadTeacherGroups(): void {
    const teacherId = sessionStorage.getItem('userId');
    if (teacherId) {
      this.http.get<GroupDTO[]>(`${environment.apiUrl}/groups/teacher/${teacherId}`)
        .subscribe({
          next: (groups) => this.teacherGroups = groups,
          error: (err) => console.error('Błąd przy pobieraniu grup nauczyciela:', err)
        });
    }
  }

  loadStudentSubjects(): void {
    const studentId = sessionStorage.getItem('userId');
    if (studentId) {
      this.http.get<SubjectDTO[]>(`${environment.apiUrl}/grades/student/${studentId}/subjects`)
        .subscribe({
          next: (subjects) => this.studentSubjects = subjects,
          error: (err) => console.error('Błąd przy pobieraniu przedmiotów ucznia:', err)
        });
    }
  }

  goToAddGrade() {
    this.navigateTo.emit('add-grade');
  }

  goToGroupGrades(groupId: number) {
    this.navigateTo.emit({ view: 'group-grades', groupId });
  }

  showSubjectGrades(subjectId: number) {
    this.activeSubjectId = subjectId;
  }

  closeSubjectGrades() {
    this.activeSubjectId = null;
  }
}
