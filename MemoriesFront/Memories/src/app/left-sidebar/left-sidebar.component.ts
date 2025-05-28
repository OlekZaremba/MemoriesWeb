import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-left-sidebar',
  standalone: true,
  imports: [
    RouterLink,
    LucideAngularModule,
    NgIf
  ],
  templateUrl: './left-sidebar.component.html',
  styleUrl: './left-sidebar.component.css'
})
export class LeftSidebarComponent implements OnInit {
  @Output() viewChange = new EventEmitter<string>();
  activeButton: string = 'glowna';
  userRole: string | null = null;

  ngOnInit(): void {
    this.userRole = sessionStorage.getItem('userRole');
  }

  setActive(view: string) {
    if (this.activeButton !== view) {
      this.activeButton = view;
      this.viewChange.emit(view);
    } else {
      this.activeButton = view;
    }
  }
}
