import {Component, EventEmitter, Output} from '@angular/core';
import {RouterLink} from '@angular/router';
import {LucideAngularModule} from 'lucide-angular';

@Component({
  selector: 'app-left-sidebar',
  standalone: true,
  imports: [
    RouterLink,
    LucideAngularModule
  ],
  templateUrl: './left-sidebar.component.html',
  styleUrl: './left-sidebar.component.css'
})
export class LeftSidebarComponent {
  @Output() viewChange = new EventEmitter<string>();
  activeButton: string = 'glowna';

  setActive(view: string) {
    this.activeButton = view;
    this.viewChange.emit(view);
  }
}

