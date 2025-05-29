import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgForOf, NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';

const BASE_URL = 'http://localhost:5017';

interface Lesson {
  id: number;
  assignmentId: number;
  lessonDate: string;
  startTime: string;
  endTime: string;
  teacherName: string;
  subjectName: string;
  groupName: string;
}

interface Assignment {
  assignmentId: number;
  teacherName: string;
  subjectName: string;
}

interface ClassGroup {
  id: number;
  groupName: string;
}

@Component({
  selector: 'app-schedule',
  standalone: true,
  imports: [FormsModule, NgIf, NgForOf],
  templateUrl: './schedule.component.html',
  styleUrl: './schedule.component.css',
})
export class ScheduleComponent implements OnInit {

  isAdmin: boolean = false;

  selectedDate = this.formatDate(new Date());
  showEditModal = false;

  lessons: Lesson[] = [];

  availableClasses: ClassGroup[] = [];
  availableAssignments: Assignment[] = [];

  editLesson = {
    classId: null as number | null,
    assignmentId: null as number | null,
    date: this.formatDate(new Date()),
    startTime: '',
    endTime: ''
  };

  private http = inject(HttpClient);

  ngOnInit(): void {
    const role = sessionStorage.getItem('userRole');
    this.isAdmin = role === 'A';
    this.loadLessonsForDate(this.selectedDate);
  }

  formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  onDateChange() {
    this.loadLessonsForDate(this.selectedDate);
  }

  loadLessonsForDate(date: string): void {
    const groupId = sessionStorage.getItem('groupId');
    const from = date;
    const to = date;

    this.http.get<Lesson[]>(`${BASE_URL}/api/schedules/group/${groupId}?from=${from}&to=${to}`)
      .subscribe(data => {
        this.lessons = data;
      });
  }

  getLessonsForSelectedDate(): Lesson[] {
    return this.lessons;
  }

  openEditModal() {
    this.loadAvailableClasses();
    this.availableAssignments = []; // reset assignments
    this.editLesson = {
      classId: null,
      assignmentId: null,
      date: this.formatDate(new Date()),
      startTime: '',
      endTime: ''
    };
    this.showEditModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
  }

  onClassChange() {
    if (this.editLesson.classId != null) {
      this.http.get<Assignment[]>(`${BASE_URL}/api/groups/group/${this.editLesson.classId}/assignments`)
        .subscribe(data => {
          console.log("📦 Otrzymane assignmenty:", data);
          this.availableAssignments = data;
        });
    } else {
      this.availableAssignments = [];
    }
    this.editLesson.assignmentId = null;
  }

  saveLesson() {
    const payload = {
      assignmentId: Number(this.editLesson.assignmentId),
      lessonDate: this.editLesson.date,
      startTime: this.editLesson.startTime,
      endTime: this.editLesson.endTime
    };

    console.log("Payload do wysłania:", payload);

    this.http.post(`${BASE_URL}/api/schedules`, payload)
      .subscribe({
        next: () => {
          alert("✅ Lekcja zapisana");
          this.closeEditModal();
          this.loadLessonsForDate(this.selectedDate);
        },
        error: () => {
          alert("❌ Nie udało się zapisać lekcji");
        }
      });
  }

  loadAvailableClasses() {
    this.http.get<ClassGroup[]>(`${BASE_URL}/api/groups`)
      .subscribe(data => {
        this.availableClasses = data;
      });
  }
}
