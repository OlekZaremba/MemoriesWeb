import {Component, ViewChild} from '@angular/core';
import {MenuNavbarComponent} from '../menu-navbar/menu-navbar.component';
import {LeftSidebarComponent} from '../left-sidebar/left-sidebar.component';
import {HomeComponent} from '../home/home.component';
import {GradesComponent} from '../grades/grades.component';
import {CommonModule} from '@angular/common';
import {GradeViewComponent} from '../grade-view/grade-view.component';
import {AddGradeComponent} from '../add-grade/add-grade.component';
import {GroupGradesComponent} from '../group-grades/group-grades.component';
import {UsersComponent} from '../users/users.component';
import {GroupUsersComponent} from '../group-users/group-users.component';
import {ClassesComponent} from '../classes/classes.component';
import {ScheduleComponent} from '../schedule/schedule.component';
import {SummaryComponent} from '../summary/summary.component';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    CommonModule,
    MenuNavbarComponent,
    LeftSidebarComponent,
    HomeComponent,
    GradesComponent,
    GradeViewComponent,
    AddGradeComponent,
    GroupGradesComponent,
    UsersComponent,
    GroupUsersComponent,
    ClassesComponent,
    ScheduleComponent,
    SummaryComponent
  ],
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css'
})
export class HomepageComponent {
  activeView: string = 'glowna';
  selectedGroupId: number | null = null;

  @ViewChild(LeftSidebarComponent, { static: true })
  leftSidebar!: LeftSidebarComponent;

  setActiveView(view: string) {
    console.log('Ustawiam widok na:', view, typeof view);

    if (view.startsWith('group-grades')) {
      const [, groupId] = view.split(':');
      this.activeView = 'group-grades';
      this.selectedGroupId = Number(groupId);
    } else {
      this.activeView = view;
    }

    if (this.leftSidebar) {
      if (view === 'grade-view' || view == 'add-grade' || view.startsWith('group-grades')) {
        this.leftSidebar.setActive('oceny');
      } else if (view === 'uzytkownicy' || view == 'group-users') {
        this.leftSidebar.setActive('uzytkownicy');
      } else {
        this.leftSidebar.setActive(view);
      }
    }
  }
}

