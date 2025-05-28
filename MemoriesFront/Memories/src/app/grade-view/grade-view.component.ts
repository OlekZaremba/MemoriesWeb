import { Component, Input, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { environment } from '../../environments/environment';

interface Grade {
  type: string;
  issueDate: string;
  grade: number;
  description: string;
}

@Component({
  selector: 'app-grade-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grade-view.component.html',
  styleUrl: './grade-view.component.css'
})
export class GradeViewComponent implements OnChanges {
  @Input() subjectId!: number;
  grades: Grade[] = [];

  constructor(private http: HttpClient) {}

  ngOnChanges(): void {
    const studentId = sessionStorage.getItem('userId');
    if (studentId && this.subjectId) {
      this.http.get<Grade[]>(`${environment.apiUrl}/grades/student/${studentId}/subject/${this.subjectId}`)
        .subscribe({
          next: (data) => this.grades = data,
          error: (err) => console.error('Błąd pobierania ocen przedmiotu:', err)
        });
    }
  }
}
