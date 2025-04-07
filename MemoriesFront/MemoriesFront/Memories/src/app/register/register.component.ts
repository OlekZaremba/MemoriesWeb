import { Component } from '@angular/core';
import {BackgroundComponent} from '../background/background.component';
import {LoginComponent} from '../login/login.component';
import {RouterLink, RouterLinkActive} from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    BackgroundComponent,
    LoginComponent,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

}
