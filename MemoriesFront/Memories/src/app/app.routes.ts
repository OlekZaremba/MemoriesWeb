import { Routes } from '@angular/router';
import {RegisterComponent} from './register/register.component';
import {BackgroundComponent} from './background/background.component';
import {LoginComponent} from './login/login.component';

export const routes: Routes = [
  { path: '', component: BackgroundComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent }
];
