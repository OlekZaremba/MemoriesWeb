import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CommonModule } from '@angular/common';

interface GroupDTO {
  id: number;
  groupName: string;
}

@Component({
  selector: 'app-grades',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grades.component.html',
  styleUrl: './grades.component.css'
})
export class GradesComponent implements OnInit {
  @Output() navigateTo = new EventEmitter<string>();
  userRole: string | null = null;
  teacherGroups: GroupDTO[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.userRole = sessionStorage.getItem('userRole');

    if (this.userRole === 'T') {
      this.loadTeacherGroups();
    }
  }

  loadTeacherGroups(): void {
    const teacherId = sessionStorage.getItem('userId');
    if (teacherId) {
      this.http.get<GroupDTO[]>(`${environment.apiUrl}/groups/teacher/${teacherId}`)
        .subscribe({
          next: (groups) => {
            this.teacherGroups = groups;
          },
          error: (err) => {
            console.error('Błąd przy pobieraniu grup nauczyciela:', err);
          }
        });
    }
  }

  goToGradeView() {
    this.navigateTo.emit('grade-view');
  }

  goToAddGrade() {
    this.loadTeacherGroups();
    this.navigateTo.emit('add-grade');
  }

  goToGroupGrades(groupId: number) {
    this.navigateTo.emit(`group-grades/${groupId}`);
  }
}
