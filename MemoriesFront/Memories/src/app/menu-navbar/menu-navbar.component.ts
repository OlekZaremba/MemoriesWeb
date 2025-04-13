import { Component } from '@angular/core';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-menu-navbar',
  standalone: true,
  imports: [
    RouterLink
  ],
  templateUrl: './menu-navbar.component.html',
  styleUrl: './menu-navbar.component.css'
})
export class MenuNavbarComponent {

}
