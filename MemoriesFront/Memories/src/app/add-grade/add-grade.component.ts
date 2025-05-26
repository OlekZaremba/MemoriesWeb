import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface GroupDTO {
  id: number;
  groupName: string;
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
  selectedGroupId: number | null = null;

  selectedStudent = '';
  selectedGrade = '';
  gradeType = '';
  comment = '';

  students = ['Jan Kowalski', 'Anna Nowak', 'Piotr Zieliński']; // później dynamicznie
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
    // 🟡 TODO: pobierz uczniów tej klasy z backendu, np. przez this.http.get(...)
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  saveGrade() {
    console.log('Zapisano ocenę:', {
      groupId: this.selectedGroupId,
      student: this.selectedStudent,
      grade: this.selectedGrade,
      type: this.gradeType,
      comment: this.comment
    });
    this.closeModal();
  }
}
