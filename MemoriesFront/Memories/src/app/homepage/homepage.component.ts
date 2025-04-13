import { Component } from '@angular/core';
import {MenuNavbarComponent} from '../menu-navbar/menu-navbar.component';
import {LeftSidebarComponent} from '../left-sidebar/left-sidebar.component';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    MenuNavbarComponent,
    LeftSidebarComponent
  ],
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css'
})
export class HomepageComponent {

}
