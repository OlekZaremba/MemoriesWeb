import { Component, EventEmitter, Output } from '@angular/core';
import { NgIf, NgForOf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [NgIf, NgForOf, FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent {
  @Output() navigateTo = new EventEmitter<string>();
  // MODAL: RESET HASŁA
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

  // MODAL: TWORZENIE UŻYTKOWNIKA
  showCreateModal = false;
  newUser = {
    firstName: '',
    lastName: '',
    email: '',
    role: ''
  };

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
    // tutaj dodasz wywołanie do serwisu/resetu
    this.closeResetModal();
  }

  // MODAL: PRZYPISYWANIE DO GRUP
  showAssignModal = false;
  selectedUserName = '';
  availableClasses = ['Klasa 1', 'Klasa 2', 'Klasa 3'];
  selectedClasses: string[] = [];

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
    // Tutaj wywołanie do backendu
    this.closeCreateModal();
  }

  protected readonly HTMLInputElement = HTMLInputElement;
}
