import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-grades',
  standalone: true,
  imports: [],
  templateUrl: './grades.component.html',
  styleUrl: './grades.component.css'
})
export class GradesComponent {
  @Output() navigateTo = new EventEmitter<string>();

  goToGradeView() {
    console.log('klik działa!');
    this.navigateTo.emit('grade-view');
  }

  goToAddGrade() {
    this.navigateTo.emit('add-grade');
  }

  goToGroupGrades() {
    this.navigateTo.emit('group-grades');
  }

}
