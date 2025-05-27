import { Component, Input, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

interface StudentWithGrades {
  id: number;
  name: string;
  surname: string;
  grades: Grade[];
  average: number;
}

interface Grade {
  id: number;
  value: number;
  type: string;
  comment: string;
  issueDate: string;
}

@Component({
  selector: 'app-group-grades',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './group-grades.component.html',
  styleUrl: './group-grades.component.css'
})
export class GroupGradesComponent implements OnChanges {
  @Input() groupId: number | null = null;

  students: StudentWithGrades[] = [];
  showModal = false;
  selectedGrade: Grade & { student: string } = {
    id: 0,
    value: 0,
    type: '',
    comment: '',
    issueDate: '',
    student: ''
  };

  constructor(private http: HttpClient) {}

  ngOnChanges(): void {
    if (this.groupId) {
      this.loadGroupGrades();
    }
  }

  loadGroupGrades(): void {
    this.http.get<StudentWithGrades[]>(`http://localhost:5017/api/grades/group/${this.groupId}/students`)
      .subscribe({
        next: (students) => this.students = students,
        error: (err) => console.error('Błąd ładowania ocen uczniów:', err)
      });
  }

  openModal(grade: Grade, student: string): void {
    this.selectedGrade = { ...grade, student };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }
}
