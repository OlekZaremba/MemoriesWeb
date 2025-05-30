import { Component, EventEmitter, inject, OnInit, Output, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';


const BASE_URL = 'http://localhost:5017';

interface LessonFromAPI {
  id: number;
  assignmentId: number;
  lessonDate: string;
  startTime: string;
  endTime: string;
  teacherName: string;
  subjectName: string;
  groupName: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    HttpClientModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  @Output() navigateTo = new EventEmitter<string>();

  todayLessons: LessonFromAPI[] = [];
  isLoading: boolean = true;
  errorMessage: string | null = null;

  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  ngOnInit(): void {
    this.loadTodaysLessonsForStudent();
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }

  loadTodaysLessonsForStudent(): void {
    this.isLoading = true;
    this.errorMessage = null;

    if (isPlatformBrowser(this.platformId)) {
      const groupId = sessionStorage.getItem('groupId');

      if (groupId) {
        const todayDate = this.formatDate(new Date());
        this.http.get<LessonFromAPI[]>(`${BASE_URL}/api/schedules/group/${groupId}?from=${todayDate}&to=${todayDate}`)
          .subscribe({
            next: (lessons) => {
              this.todayLessons = lessons;
              this.isLoading = false;
            },
            error: (err) => {
              console.error('Błąd podczas ładowania planu lekcji na dziś:', err);
              this.errorMessage = 'Nie udało się załadować planu lekcji. Spróbuj ponownie później.';
              this.isLoading = false;
              this.todayLessons = [];
            }
          });
      } else {
        console.warn('Nie znaleziono groupId w sessionStorage. Nie można załadować planu lekcji.');
        this.errorMessage = 'Informacje o grupie nie są dostępne. Nie można załadować planu.';
        this.isLoading = false;
        this.todayLessons = [];
      }
    } else {
      console.log('SSR: Pomijanie ładowania lekcji z sessionStorage.');
      this.isLoading = false;
    }
  }

  getTodaysLessons(): LessonFromAPI[] {
    return this.todayLessons;
  }


  goToGrades(): void { this.navigateTo.emit('oceny'); }
  goToUsers(): void { this.navigateTo.emit('uzytkownicy'); }
  goToSummary(): void { this.navigateTo.emit('wykresy'); }
  goToSchedule(): void { this.navigateTo.emit('plan'); }
}
