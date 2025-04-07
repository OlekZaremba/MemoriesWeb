import { Component } from '@angular/core';
import {LoginComponent} from "../login/login.component";

@Component({
  selector: 'app-background',
  standalone: true,
    imports: [
        LoginComponent
    ],
  templateUrl: './background.component.html',
  styleUrl: './background.component.css'
})
export class BackgroundComponent {

}
