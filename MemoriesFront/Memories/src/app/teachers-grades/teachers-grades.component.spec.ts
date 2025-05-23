import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeachersGradesComponent } from './teachers-grades.component';

describe('TeachersGradesComponent', () => {
  let component: TeachersGradesComponent;
  let fixture: ComponentFixture<TeachersGradesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeachersGradesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TeachersGradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
