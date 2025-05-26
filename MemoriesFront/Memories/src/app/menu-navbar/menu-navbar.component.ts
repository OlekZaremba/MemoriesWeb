import { Component, ElementRef, Inject, OnInit, PLATFORM_ID, ViewChild } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-menu-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './menu-navbar.component.html',
  styleUrl: './menu-navbar.component.css'
})
export class MenuNavbarComponent implements OnInit {
  @ViewChild('profileBtn') profileBtn!: ElementRef<HTMLButtonElement>;

  imageSrc: string | null = null;
  userName: string | null = null;
  userRole: string | null = null;
  isBrowser: boolean;

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    if (!this.isBrowser) return;

    if (this.shouldRender()) {
      this.userName = sessionStorage.getItem('userName');
      this.userRole = sessionStorage.getItem('userRole');

      const userId = sessionStorage.getItem('userId');
      if (userId) {
        this.getProfileImage(userId);
      }
    }
  }

  shouldRender(): boolean {
    const excludedRoutes = ['/login', '/register', '/'];
    return !excludedRoutes.includes(this.router.url);
  }

  onFileSelected(event: any): void {
    if (!this.isBrowser) return;

    const file = event.target.files[0];
    const reader = new FileReader();

    reader.onload = () => {
      this.imageSrc = reader.result as string;

      const userId = sessionStorage.getItem('userId');
      if (!userId) return;

      this.setButtonBackground(this.imageSrc);

      this.http.put(`http://localhost:5017/api/users/${userId}/profile-image`, {
        image: this.imageSrc
      }).subscribe({
        next: () => console.log('✅ Zdjęcie zapisane'),
        error: err => console.error('❌ Błąd przy zapisie zdjęcia:', err)
      });
    };

    if (file) {
      reader.readAsDataURL(file);
    }
  }

  getProfileImage(userId: string): void {
    this.http.get<{ image: string }>(`http://localhost:5017/api/users/${userId}/profile-image`)
      .subscribe({
        next: (res) => {
          if (res.image) {
            this.imageSrc = res.image;
            this.setButtonBackground(res.image);
          }
        },
        error: (err) => console.error('❌ Błąd przy pobieraniu zdjęcia:', err)
      });
  }

  setButtonBackground(base64: string): void {
    if (this.profileBtn?.nativeElement) {
      const btn = this.profileBtn.nativeElement;
      btn.style.backgroundImage = `url(${base64})`;
      btn.style.backgroundSize = 'cover';
      btn.style.backgroundPosition = 'center';
    }
  }
}
