import { Component, ViewChild, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

import { MenuNavbarComponent } from '../menu-navbar/menu-navbar.component';
import { LeftSidebarComponent } from '../left-sidebar/left-sidebar.component';
import { HomeComponent } from '../home/home.component';
import { GradesComponent } from '../grades/grades.component';
import { GradeViewComponent } from '../grade-view/grade-view.component';
import { AddGradeComponent } from '../add-grade/add-grade.component';
import { GroupGradesComponent } from '../group-grades/group-grades.component';
import { UsersComponent } from '../users/users.component';
import { GroupUsersComponent } from '../group-users/group-users.component';
import { ClassesComponent } from '../classes/classes.component';
import { ScheduleComponent } from '../schedule/schedule.component';
import { SummaryComponent } from '../summary/summary.component';

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
export class HomepageComponent implements OnInit {
  activeView: string = 'glowna';
  selectedGroupId: number | null = null;

  @ViewChild(LeftSidebarComponent, { static: true })
  leftSidebar!: LeftSidebarComponent;

  private router = inject(Router);

  ngOnInit(): void {
    if (typeof window !== 'undefined') {
      const userId = sessionStorage.getItem('userId');
      if (!userId) {
        this.router.navigateByUrl('/', { replaceUrl: true });
      }
    }
  }

  setActiveView(view: string | { view: string, groupId?: number }) {
    if (typeof view === 'string') {
      this.activeView = view;
      this.selectedGroupId = null;
    } else {
      this.activeView = view.view;

      if (view.view === 'group-users' || view.view === 'group-grades') {
        this.selectedGroupId = view.groupId ?? null;
      } else {
        this.selectedGroupId = null;
      }
    }

    if (this.leftSidebar) {
      if (
        this.activeView === 'grade-view' ||
        this.activeView === 'add-grade' ||
        this.activeView === 'group-grades'
      ) {
        this.leftSidebar.setActive('oceny');
      } else if (this.activeView === 'uzytkownicy' || this.activeView === 'group-users') {
        this.leftSidebar.setActive('uzytkownicy');
      } else {
        this.leftSidebar.setActive(this.activeView);
      }
    }
  }
}
