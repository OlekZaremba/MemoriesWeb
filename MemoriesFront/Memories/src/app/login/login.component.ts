import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private http: HttpClient, private router: Router) {}

  login() {
    const body = {
      login: this.email,
      password: this.password
    };

    console.log('Wysyłam dane logowania:', body); // Dodaj ten log

    this.http.post<any>(`${environment.apiUrl}/auth/login`, body).subscribe({
      next: (resp) => {
        sessionStorage.setItem('userId', resp.id);
        sessionStorage.setItem('userName', `${resp.name} ${resp.surname}`);
        sessionStorage.setItem('userRole', resp.role);
        sessionStorage.setItem('userImage', resp.image ?? '');
        sessionStorage.setItem('userClassName', resp.className ?? '');

        this.router.navigate(['/homepage']);
      },
      error: () => {
        this.errorMessage = 'Nieprawidłowy login lub hasło';
      }
    });
  }

}
