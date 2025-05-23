import {Component, ViewChild} from '@angular/core';
import {MenuNavbarComponent} from '../menu-navbar/menu-navbar.component';
import {LeftSidebarComponent} from '../left-sidebar/left-sidebar.component';
import {HomeComponent} from '../home/home.component';
import {GradesComponent} from '../grades/grades.component';
import {CommonModule} from '@angular/common';
import {GradeViewComponent} from '../grade-view/grade-view.component';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [
    CommonModule,
    MenuNavbarComponent,
    LeftSidebarComponent,
    HomeComponent,
    GradesComponent,
    GradeViewComponent
  ],
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css'
})
export class HomepageComponent {
  activeView: string = 'glowna';

  @ViewChild(LeftSidebarComponent, { static: true })
  leftSidebar!: LeftSidebarComponent;

  setActiveView(view: any) {
    console.log('Ustawiam widok na:', view, typeof view);
    this.activeView = view;

    if (this.leftSidebar) {
      this.leftSidebar.setActive('oceny');
    }
  }

}
