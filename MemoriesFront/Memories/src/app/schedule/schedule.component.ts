import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-schedule',
  standalone: true,
  imports: [FormsModule, NgIf, NgForOf],
  templateUrl: './schedule.component.html',
  styleUrl: './schedule.component.css',
})
export class ScheduleComponent {
  selectedDate = this.formatDate(new Date());
  showEditModal = false;

  lessons = [
    {
      date: this.selectedDate, // DZIŚ
      subject: 'Matematyka',
      teacher: 'Jan Kowalski',
      startTime: '08:00',
      endTime: '08:45',
    },
    {
      date: this.selectedDate, // DZIŚ
      subject: 'Fizyka',
      teacher: 'Anna Nowak',
      startTime: '09:00',
      endTime: '09:45',
    },
    {
      date: this.selectedDate, // DZIŚ
      subject: 'Informatyka',
      teacher: 'Tomasz Zięba',
      startTime: '10:00',
      endTime: '10:45',
    },
  ];

  availableClasses = ['Klasa 1A', 'Klasa 2B', 'Klasa 3C'];

  availableTeachers = [
    'Jan Kowalski - Matematyka',
    'Anna Nowak - Fizyka',
    'Tomasz Zięba - Biologia'
  ];

  editLesson = {
    class: '',
    teacher: '',
    date: this.formatDate(new Date()),
    startTime: '',
    endTime: ''
  };

  saveLesson() {
    console.log('💾 Zapisuję nową lekcję:', this.editLesson);
    this.closeEditModal();
  }


  formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  getLessonsForSelectedDate() {
    return this.lessons.filter(lesson => lesson.date === this.selectedDate);
  }

  openEditModal() {
    this.showEditModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
  }
}
