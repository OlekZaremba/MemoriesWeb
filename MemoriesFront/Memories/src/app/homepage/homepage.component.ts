import { Component } from '@angular/core';
import {MenuNavbarComponent} from '../menu-navbar/menu-navbar.component';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    MenuNavbarComponent
  ],
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css'
})
export class HomepageComponent {

}
