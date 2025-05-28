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
  @Output() navigateTo = new EventEmitter<string>();

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
    role: ''
  };

  showAssignModal = false;
  selectedUserName = '';
  availableClasses = ['Klasa 1', 'Klasa 2', 'Klasa 3'];
  selectedClasses: string[] = [];

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
    }
  }

  loadTeachersForStudent() {
    this.http.get<any[]>(`${BASE_URL}/api/users/student/${this.userId}/teachers`)
      .subscribe(data => {
        this.teachersForStudent = data;
      });
  }

  loadGroupsForTeacher() {
    this.http.get<any[]>(`${BASE_URL}/api/users/${this.userId}/groups`)
      .subscribe(data => {
        this.teacherGroups = data;
      });
  }

  loadAllUsers() {
    this.http.get<any[]>(`${BASE_URL}/api/users`)
      .subscribe(data => {
        this.allUsers = data;
      });
  }

  goToGroupUsersView() {
    this.navigateTo.emit('group-users');
  }

  openResetModal(email: string) {
    this.selectedEmail = email;
    this.showResetModal = true;
  }

  closeResetModal() {
    this.showResetModal = false;
  }

  resetPassword() {
    console.log(`Resetuję hasło dla: ${this.selectedEmail}`);
    this.closeResetModal();
  }

  openAssignModal(userName: string) {
    this.selectedUserName = userName;
    this.selectedClasses = [];
    this.showAssignModal = true;
  }

  closeAssignModal() {
    this.showAssignModal = false;
  }

  toggleGroup(group: string, checked: boolean) {
    if (checked) {
      this.selectedClasses.push(group);
    } else {
      this.selectedClasses = this.selectedClasses.filter(g => g !== group);
    }
  }

  assignToGroups() {
    console.log(`✅ Przypisuję ${this.selectedUserName} do klas:`, this.selectedClasses);
    this.closeAssignModal();
  }

  openEditModal(user: any) {
    this.editUser = { ...user };
    this.showEditModal = true;
  }

  prepareUserForEdit(user: any) {
    const editData = {
      firstName: user.name,
      lastName: user.surname,
      email: '',
      role: user.role,
      groups: ''
    };
    this.openEditModal(editData);
  }

  closeEditModal() {
    this.showEditModal = false;
  }

  saveUserEdits() {
    console.log('💾 Zapisano dane użytkownika:', this.editUser);
    this.closeEditModal();
  }

  openCreateModal() {
    this.newUser = {
      firstName: '',
      lastName: '',
      email: '',
      role: 'Uczeń'
    };
    this.showCreateModal = true;
  }

  closeCreateModal() {
    this.showCreateModal = false;
  }

  createUser() {
    console.log('👤 Tworzenie użytkownika:', this.newUser);
    this.closeCreateModal();
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
