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


interface SubjectDetailsDTO {
  id: number;
  className: string;

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
  currentSubjectForGrading: SubjectDetailsDTO | null = null;
  selectedStudentId: number | null = null;
  selectedGrade = '';
  gradeType = '';
  comment = '';

  grades = ['1', '2-', '2', '2+', '3-', '3', '3+', '4-', '4', '4+', '5-', '5', '5+', '6'];
  isLoadingSubject = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    const teacherId = sessionStorage.getItem('userId');
    if (teacherId) {
      this.http.get<GroupDTO[]>(`${environment.apiUrl}/groups/teacher/${teacherId}`)
        .subscribe({
          next: (data) => this.groups = data,
          error: (err) => console.error('Błąd ładowania grup nauczyciela:', err)
        });
    }
  }

  openModal(groupId: number): void {
    this.selectedGroupId = groupId;
    this.currentSubjectForGrading = null;
    this.students = [];
    this.selectedStudentId = null;
    this.selectedGrade = '';
    this.gradeType = '';
    this.comment = '';
    this.isLoadingSubject = true;
    this.showModal = true;

    const teacherId = sessionStorage.getItem('userId');

    if (!teacherId) {
      alert("Nie można zidentyfikować nauczyciela. Zaloguj się ponownie.");
      this.isLoadingSubject = false;
      this.closeModal();
      return;
    }


    this.http.get<SubjectDetailsDTO>(`${environment.apiUrl}/groups/${groupId}/teachers/${teacherId}/subject`)
      .subscribe({
        next: (subjectData) => {
          if (subjectData && subjectData.id) {
            this.currentSubjectForGrading = subjectData;

            this.http.get<StudentDTO[]>(`${environment.apiUrl}/grades/group/${groupId}/students`)
              .subscribe({
                next: (studentData) => {
                  this.students = studentData;
                  this.isLoadingSubject = false;
                },
                error: (err) => {
                  console.error(`Błąd ładowania uczniów dla grupy ${groupId}:`, err);
                  alert(`Nie udało się załadować listy uczniów dla grupy.`);
                  this.isLoadingSubject = false;
                }
              });
          } else {
            console.error('Nie znaleziono przedmiotu dla nauczyciela w tej grupie lub odpowiedź API jest niekompletna.');
            alert('Nie można określić przedmiotu dla tej grupy. Sprawdź, czy nauczyciel jest poprawnie przypisany do przedmiotu w tej grupie.');
            this.isLoadingSubject = false;


          }
        },
        error: (err) => {
          console.error(`Błąd ładowania przedmiotu dla nauczyciela ${teacherId} w grupie ${groupId}:`, err);
          alert('Wystąpił błąd podczas pobierania informacji o przedmiocie. Upewnij się, że backend działa i nauczyciel ma przypisany przedmiot w tej grupie.');
          this.isLoadingSubject = false;

        }
      });
  }

  closeModal(): void {
    this.showModal = false;
    this.selectedGroupId = null;
    this.currentSubjectForGrading = null;
    this.selectedStudentId = null;
    this.selectedGrade = '';
    this.gradeType = '';
    this.comment = '';
    this.students = [];
    this.isLoadingSubject = false;
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

  saveGrade(): void {
    if (!this.selectedStudentId || !this.selectedGrade || !this.gradeType.trim()) {
      alert("Uzupełnij wszystkie wymagane pola: uczeń, ocena, typ oceny.");
      return;
    }

    const teacherIdString = sessionStorage.getItem('userId');
    if (!teacherIdString) {
      alert("Nie można zidentyfikować nauczyciela. Zaloguj się ponownie.");
      return;
    }

    if (!this.currentSubjectForGrading || !this.currentSubjectForGrading.id) {
      alert("Nie udało się określić przedmiotu. Nie można zapisać oceny.");
      return;
    }

    const payload = {
      studentId: this.selectedStudentId,
      teacherId: parseInt(teacherIdString),
      classId: this.currentSubjectForGrading.id,
      grade: this.convertGrade(this.selectedGrade),
      type: this.gradeType.trim(),
      description: this.comment.trim()
    };

    console.log('Payload wysyłany do API:', payload);

    this.http.post(`${environment.apiUrl}/grades`, payload).subscribe({
      next: () => {
        alert('Ocena została zapisana.');
        this.closeModal();
      },
      error: (err) => {
        console.error('Błąd zapisu oceny:', err);
        alert("Wystąpił błąd podczas zapisu oceny. Sprawdź konsolę po więcej informacji.");
      }
    });
  }
}
