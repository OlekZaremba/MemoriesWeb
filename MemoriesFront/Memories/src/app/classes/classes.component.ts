import { Component, OnInit, inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgIf, NgForOf, CommonModule, isPlatformBrowser } from '@angular/common'; // Dodano CommonModule i isPlatformBrowser
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
  groupMemberId: number;
  teacherName: string;
  groupName: string;
}

// DTO dla tworzenia grupy, zgodnie z GroupController.cs
interface CreateGroupRequest {
  groupName: string;
}

// DTO dla odpowiedzi po utworzeniu grupy, zgodnie z GroupController.cs
interface GroupDTO {
  id: number;
  groupName: string;
}


@Component({
  selector: 'app-classes',
  standalone: true,
  imports: [CommonModule, FormsModule], // CommonModule zastępuje NgIf, NgForOf
  templateUrl: './classes.component.html',
  styleUrl: './classes.component.css'
})
export class ClassesComponent implements OnInit {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  isAdmin: boolean = false; // Flaga dla roli admina

  showAddSubjectModal = false;
  showAssignTeacherModal = false;
  showAddGroupModal = false; // NOWA FLAGA MODALA

  newSubjectName: string = '';
  newGroupName: string = ''; // NOWA WŁAŚCIWOŚĆ DLA NAZWY GRUPY

  subjects: SubjectDTO[] = [];
  groupMembers: GroupMemberDTO[] = [];

  selectedSubjectId: number | null = null;
  selectedGroupMemberId: number | null = null;

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) { // Sprawdzanie platformy przed dostępem do sessionStorage
      const role = sessionStorage.getItem('userRole');
      this.isAdmin = role === 'A';
    }
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

  // --- Zarządzanie Przedmiotami ---
  openAddSubjectModal() {
    this.newSubjectName = '';
    this.showAddSubjectModal = true;
  }

  closeAddSubjectModal() {
    this.showAddSubjectModal = false;
  }

  addSubject() {
    const name = this.newSubjectName.trim();
    if (!name) {
      alert("Nazwa przedmiotu nie może być pusta.");
      return;
    }
    this.http.post(`${environment.apiUrl}/classes`, { className: name }).subscribe({
      next: () => {
        alert("Przedmiot dodany pomyślnie.");
        this.loadSubjects();
        this.closeAddSubjectModal();
      },
      error: err => {
        console.error('Błąd dodawania przedmiotu:', err);
        alert("Wystąpił błąd podczas dodawania przedmiotu.");
      }
    });
  }

  openAssignTeacherModal(subjectId: number) {
    this.selectedSubjectId = subjectId;
    this.selectedGroupMemberId = null; // Resetuj wybór
    if (this.groupMembers.length === 0) {
      alert("Brak dostępnych relacji nauczyciel-grupa do przypisania. Najpierw zdefiniuj nauczycieli i grupy oraz ich powiązania.");
    }
    this.showAssignTeacherModal = true;
  }

  closeAssignTeacherModal() {
    this.showAssignTeacherModal = false;
  }

  assignSubjectToGroupMember() {
    if (!this.selectedGroupMemberId || !this.selectedSubjectId) {
      alert('Nie wybrano nauczyciela/grupy lub przedmiotu.');
      return;
    }

    this.http.post(`${environment.apiUrl}/group-members/${this.selectedGroupMemberId}/class/${this.selectedSubjectId}`, {})
      .subscribe({
        next: () => {
          alert('Przypisano przedmiot do nauczyciela w grupie.');
          this.closeAssignTeacherModal();
          this.loadSubjects();
        },
        error: err => {
          console.error('Błąd przypisania przedmiotu:', err);
          alert("Wystąpił błąd podczas przypisywania przedmiotu.");
        }
      });
  }

  // --- NOWE METODY: Zarządzanie Grupami ---
  openAddGroupModal() {
    this.newGroupName = '';
    this.showAddGroupModal = true;
  }

  closeAddGroupModal() {
    this.showAddGroupModal = false;
  }

  addGroup() {
    const name = this.newGroupName.trim();
    if (!name) {
      alert("Nazwa grupy nie może być pusta.");
      return;
    }

    const payload: CreateGroupRequest = { groupName: name };

    // Zmieniony endpoint - zakładamy, że environment.apiUrl zawiera już /api
    // np. environment.apiUrl = "http://localhost:5017/api"
    // wtedy pełny URL do GroupController będzie "http://localhost:5017/api/groups"
    this.http.post<GroupDTO>(`${environment.apiUrl}/groups`, payload).subscribe({
      next: (newGroup) => {
        alert(`Grupa "${newGroup.groupName}" dodana pomyślnie.`);
        this.closeAddGroupModal();
        this.loadGroupMembers();
      },
      error: err => {
        console.error('Błąd dodawania grupy:', err);
        alert("Wystąpił błąd podczas dodawania grupy. Sprawdź konsolę przeglądarki (F12) po więcej szczegółów.");
      }
    });
  }
}
