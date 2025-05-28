import { Component, EventEmitter, Output, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';


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
  private router = inject(Router);

  ngOnInit(): void {
    if (typeof window !== 'undefined') {
      this.userRole = sessionStorage.getItem('userRole');
    }
  }

  setActive(view: string) {
    if (this.activeButton !== view) {
      this.activeButton = view;
      this.viewChange.emit(view);
    } else {
      this.activeButton = view;
    }
  }

  logout(): void {
    sessionStorage.clear();
    this.router.navigateByUrl('/', { replaceUrl: true });
  }
}
