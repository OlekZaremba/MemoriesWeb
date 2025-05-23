import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupGradesComponent } from './group-grades.component';

describe('GroupGradesComponent', () => {
  let component: GroupGradesComponent;
  let fixture: ComponentFixture<GroupGradesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupGradesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupGradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
