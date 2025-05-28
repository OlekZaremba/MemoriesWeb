import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from "@angular/forms";
import { NgForOf, NgIf } from "@angular/common";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-group-users',
  standalone: true,
  imports: [
    FormsModule,
    NgForOf,
    NgIf
  ],
  templateUrl: './group-users.component.html',
  styleUrl: './group-users.component.css'
})
export class GroupUsersComponent implements OnChanges {
  @Input() groupId: number | null = null;
  students: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['groupId'] && this.groupId !== null) {
      this.loadStudents();
    }
  }

  loadStudents() {
    this.http.get<any[]>(`http://localhost:5017/api/groups/${this.groupId}/students`)
      .subscribe(data => {
        this.students = data;
      });
  }
}
