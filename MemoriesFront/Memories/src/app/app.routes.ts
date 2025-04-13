import { Routes } from '@angular/router';
import {RegisterComponent} from './register/register.component';
import {BackgroundComponent} from './background/background.component';
import {LoginComponent} from './login/login.component';
import {HomepageComponent} from './homepage/homepage.component';

export const routes: Routes = [
  { path: '', component: BackgroundComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'homepage', component: HomepageComponent}
];
