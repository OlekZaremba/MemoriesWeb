import { Component } from '@angular/core';
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
  activeButton: string = 'glowna'; // domyślnie aktywny

  setActive(button: string) {
    this.activeButton = button;

    // Możesz też tutaj podmieniać komponent lub logikę
    // np. this.loadComponent(button);
  }
}
