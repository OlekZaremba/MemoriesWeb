import {Component, EventEmitter, Output} from '@angular/core';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NgForOf],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  @Output() navigateTo = new EventEmitter<string>();

  todayLessons = [
    { subject: 'Matematyka', teacher: 'Jan Kowalski', startTime: '08:00', endTime: '08:45' },
    { subject: 'Fizyka', teacher: 'Anna Nowak', startTime: '09:00', endTime: '09:45' },
    { subject: 'Informatyka', teacher: 'Tomasz Zięba', startTime: '10:00', endTime: '10:45' },
  ];

  getTodaysLessons() {
    return this.todayLessons;
  }

  goToGrades() {
    this.navigateTo.emit('oceny');
  }

  goToUsers() {
    this.navigateTo.emit('uzytkownicy');
  }

  goToSummary() {
    this.navigateTo.emit('wykresy');
  }

  goToSchedule() {
    this.navigateTo.emit('plan');
  }
}
