import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

const BASE_URL = 'http://localhost:5017';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  email: string = '';
  private http = inject(HttpClient);

  requestPasswordReset() {
    if (!this.email) {
      alert("Wpisz adres e-mail.");
      return;
    }

    this.http.post(`${BASE_URL}/api/auth/request-password-reset`, { login: this.email })
      .subscribe({
        next: () => {
          alert("Jeśli konto istnieje, wysłano link do resetu hasła.");
        },
        error: (err) => {
          console.error("❌ Błąd resetowania hasła:", err);
          alert("Wystąpił błąd przy resetowaniu hasła.");
        }
      });
  }
}
