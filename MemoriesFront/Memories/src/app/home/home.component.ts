import {Component, EventEmitter, Output} from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  @Output() navigateTo = new EventEmitter<string>();

  goToGrades() {
    this.navigateTo.emit('oceny');
  }
}
