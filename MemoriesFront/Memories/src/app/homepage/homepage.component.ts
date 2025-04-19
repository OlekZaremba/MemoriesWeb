import { Component } from '@angular/core';
import {MenuNavbarComponent} from '../menu-navbar/menu-navbar.component';
import {LeftSidebarComponent} from '../left-sidebar/left-sidebar.component';
import {HomeComponent} from '../home/home.component';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    MenuNavbarComponent,
    LeftSidebarComponent,
    HomeComponent
  ],
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css'
})
export class HomepageComponent {

}
