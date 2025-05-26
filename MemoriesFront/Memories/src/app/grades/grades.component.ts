import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-grades',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './grades.component.html',
  styleUrl: './grades.component.css'
})
export class GradesComponent implements OnInit {
  @Output() navigateTo = new EventEmitter<string>();
  userRole: string | null = null;

  ngOnInit(): void {
    this.userRole = sessionStorage.getItem('userRole');
  }

  goToGradeView() {
    this.navigateTo.emit('grade-view');
  }

  goToAddGrade() {
    this.navigateTo.emit('add-grade');
  }

  goToGroupGrades() {
    this.navigateTo.emit('group-grades');
  }
}
