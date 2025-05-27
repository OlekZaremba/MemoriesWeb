import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface GroupDTO {
  id: number;
  groupName: string;
}

interface StudentDTO {
  id: number;
  name: string;
  surname: string;
}

@Component({
  selector: 'app-add-grade',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-grade.component.html',
  styleUrl: './add-grade.component.css'
})
export class AddGradeComponent implements OnInit {
  showModal = false;

  groups: GroupDTO[] = [];
  students: StudentDTO[] = [];

  selectedGroupId: number | null = null;
  selectedStudentId: number | null = null;
  selectedGrade = '';
  gradeType = '';
  comment = '';

  grades = ['1', '2-', '2', '2+', '3-', '3', '3+', '4-', '4', '4+', '5-', '5', '5+', '6'];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    const userId = sessionStorage.getItem('userId');
    if (userId) {
      this.http.get<GroupDTO[]>(`${environment.apiUrl}/groups/teacher/${userId}`)
        .subscribe({
          next: (data) => this.groups = data,
          error: (err) => console.error('Błąd ładowania grup nauczyciela:', err)
        });
    }
  }

  openModal(groupId: number) {
    this.selectedGroupId = groupId;
    this.http.get<StudentDTO[]>(`${environment.apiUrl}/grades/group/${groupId}/students`)
      .subscribe({
        next: (data) => this.students = data,
        error: (err) => console.error('Błąd ładowania uczniów grupy:', err)
      });
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.selectedStudentId = null;
    this.selectedGrade = '';
    this.gradeType = '';
    this.comment = '';
  }

  convertGrade(value: string): number {
    const gradeMap: { [key: string]: number } = {
      '1': 1,
      '2-': 1.75, '2': 2, '2+': 2.5,
      '3-': 2.75, '3': 3, '3+': 3.5,
      '4-': 3.75, '4': 4, '4+': 4.5,
      '5-': 4.75, '5': 5, '5+': 5.5,
      '6': 6
    };
    return gradeMap[value] ?? 0;
  }

  saveGrade() {
    if (!this.selectedStudentId || !this.selectedGrade || !this.gradeType.trim()) {
      alert("Uzupełnij wszystkie pola.");
      return;
    }

    const teacherId = sessionStorage.getItem('userId');
    if (!teacherId || !this.selectedGroupId) {
      alert("Brakuje danych o nauczycielu lub klasie.");
      return;
    }

    const payload = {
      studentId: this.selectedStudentId,
      teacherId: parseInt(teacherId),
      classId: this.selectedGroupId,
      grade: this.convertGrade(this.selectedGrade),
      type: this.gradeType,
      description: this.comment
    };

    console.log('Payload wysyłany do API:', payload);

    this.http.post(`${environment.apiUrl}/grades`, payload).subscribe({
      next: () => {
        alert('Ocena została zapisana.');
        this.closeModal();
      },
      error: (err) => {
        console.error('Błąd zapisu oceny:', err);
        alert("Wystąpił błąd podczas zapisu.");
      }
    });
  }
}
