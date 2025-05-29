import { Component, EventEmitter, Output, OnInit, inject } from '@angular/core';
import { NgIf, NgForOf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

const BASE_URL = 'http://localhost:5017';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [NgIf, NgForOf, FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  @Output() navigateTo = new EventEmitter<{ view: string, groupId: number }>();

  showResetModal = false;
  selectedEmail: string = '';

  showEditModal = false;
  editUser = {
    firstName: '',
    lastName: '',
    email: '',
    role: '',
    groups: ''
  };

  showCreateModal = false;
  newUser = {
    firstName: '',
    lastName: '',
    email: '',
    role: 'Uczeń',
    groupId: null as number | null,
    login: ''
  };

  showAssignModal = false;
  selectedUserName = '';
  selectedClasses: string[] = [];
  editedUserId: number | null = null;

  availableGroups: { id: number, groupName: string }[] = [];

  userRole: string | null = null;
  userId: number = 0;

  teachersForStudent: any[] = [];
  teacherGroups: any[] = [];
  allUsers: any[] = [];

  searchTerm: string = '';

  private http = inject(HttpClient);

  ngOnInit(): void {
    this.userRole = sessionStorage.getItem('userRole');
    this.userId = Number(sessionStorage.getItem('userId'));

    if (this.userRole === 'S') {
      this.loadTeachersForStudent();
    } else if (this.userRole === 'T') {
      this.loadGroupsForTeacher();
    } else if (this.userRole === 'A') {
      this.loadAllUsers();
      this.loadAvailableGroups();
    }
  }

  loadTeachersForStudent() {
    this.http.get<any[]>(`${BASE_URL}/api/users/student/${this.userId}/teachers`)
      .subscribe(data => this.teachersForStudent = data);
  }

  loadGroupsForTeacher() {
    this.http.get<any[]>(`${BASE_URL}/api/users/${this.userId}/groups`)
      .subscribe(data => this.teacherGroups = data);
  }

  loadAllUsers() {
    this.http.get<any[]>(`${BASE_URL}/api/users`)
      .subscribe(data => this.allUsers = data);
  }

  loadAvailableGroups() {
    this.http.get<{ id: number, groupName: string }[]>(`${BASE_URL}/api/groups`)
      .subscribe(data => this.availableGroups = data);
  }

  goToGroupUsersView(groupId: number) {
    this.navigateTo.emit({ view: 'group-users', groupId });
  }

  openResetModal(email: string) {
    this.selectedEmail = email;
    this.showResetModal = true;
  }

  closeResetModal() {
    this.showResetModal = false;
  }

  resetPassword() {
    if (!this.selectedEmail) {
      alert("Brak adresu e-mail.");
      return;
    }

    this.http.post(`${BASE_URL}/api/auth/request-password-reset`, {
      login: this.selectedEmail
    }, { responseType: 'text' }).subscribe({
      next: () => {
        alert("Link resetujący został wysłany.");
        this.closeResetModal();
      },
      error: (err) => {
        console.error("❌ Błąd resetowania hasła:", err);
        alert("Nie udało się zresetować hasła.");
      }
    });
  }

  openAssignModal(user: any) {
    this.selectedUserName = `${user.name} ${user.surname}`;
    this.selectedClasses = [];
    this.showAssignModal = true;
    this.editedUserId = user.id;

    this.http.get<{ id: number, groupName: string }[]>(`${BASE_URL}/api/users/${user.id}/available-groups`)
      .subscribe(data => this.availableGroups = data);
  }

  closeAssignModal() {
    this.showAssignModal = false;
  }

  toggleGroup(groupId: number, checked: boolean) {
    if (checked) {
      this.selectedClasses.push(groupId.toString());
    } else {
      this.selectedClasses = this.selectedClasses.filter(id => id !== groupId.toString());
    }
  }

  assignToGroups() {
    if (!this.editedUserId || this.selectedClasses.length === 0) return;

    const groupIds = this.selectedClasses.map(id => parseInt(id));

    this.http.post(`${BASE_URL}/api/users/${this.editedUserId}/assign-groups`, { groupIds })
      .subscribe({
        next: () => {
          alert('✅ Przypisano użytkownika do grup.');
          this.loadAllUsers();
          this.closeAssignModal();
        },
        error: () => {
          alert('❌ Błąd przypisywania użytkownika do grup.');
        }
      });
  }

  loadUserDetails(userId: number) {
    this.http.get<any>(`${BASE_URL}/api/users/${userId}`).subscribe(user => {
      this.editUser = {
        firstName: user.name,
        lastName: user.surname,
        email: user.login,
        role: this.mapRole(user.role),
        groups: ''
      };
      this.editedUserId = user.id;
      this.showEditModal = true;
    });
  }

  openEditModal(user: any) {
    this.editUser = { ...user };
    this.showEditModal = true;
  }

  prepareUserForEdit(user: any) {
    this.loadUserDetails(user.id);
  }

  closeEditModal() {
    this.showEditModal = false;
  }

  saveUserEdits() {
    if (!this.editedUserId) {
      console.error("❌ Brak ID użytkownika do edycji.");
      return;
    }

    const roleMap: Record<string, string> = {
      "Uczeń": "S",
      "Nauczyciel": "T",
      "Admin": "A"
    };

    const payload = {
      login: this.editUser.email,
      email: this.editUser.email,
      name: this.editUser.firstName,
      surname: this.editUser.lastName,
      role: roleMap[this.editUser.role]
    };

    this.http.put(`${BASE_URL}/api/users/${this.editedUserId}`, payload, { responseType: 'text' }).subscribe({
      next: () => {
        alert("✅ Dane użytkownika zostały zaktualizowane.");
        this.closeEditModal();
        this.loadAllUsers();
      },
      error: (err) => {
        console.error("❌ Błąd aktualizacji użytkownika:", err);
        alert("❌ Nie udało się zaktualizować użytkownika.");
      }
    });
  }

  openCreateModal() {
    this.newUser = {
      firstName: '',
      lastName: '',
      email: '',
      role: 'Uczeń',
      groupId: null,
      login: ''
    };
    this.showCreateModal = true;
  }

  closeCreateModal() {
    this.showCreateModal = false;
  }

  onCheckboxChange(event: Event, groupId: number) {
    const input = event.target as HTMLInputElement | null;

    if (input) {
      this.toggleGroup(groupId, input.checked);
    }
  }

  createUser() {
    const roleMap: Record<string, string> = {
      "Uczeń": "S",
      "Nauczyciel": "T",
      "Admin": "A"
    };

    this.newUser.login = this.newUser.email;

    const payload = {
      name: this.newUser.firstName,
      surname: this.newUser.lastName,
      login: this.newUser.login,
      email: this.newUser.email,
      password: "test123",
      role: roleMap[this.newUser.role],
      groupId: this.newUser.groupId
    };

    this.http.post(`${BASE_URL}/api/auth/register`, payload).subscribe({
      next: () => {
        alert('Użytkownik został utworzony');
        this.closeCreateModal();
        this.loadAllUsers();
      },
      error: (err) => {
        console.error('❌ Błąd przy tworzeniu użytkownika:', err);
        alert('Nie udało się utworzyć użytkownika.');
      }
    });
  }

  filteredUsers() {
    return this.allUsers.filter(u =>
      `${u.name} ${u.surname}`.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  mapRole(role: string): string {
    switch (role) {
      case 'S': return 'Uczeń';
      case 'T': return 'Nauczyciel';
      case 'A': return 'Admin';
      default: return '';
    }
  }

  protected readonly HTMLInputElement = HTMLInputElement;
}
