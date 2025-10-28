import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeatPlan } from './seat-plan';

describe('SeatPlan', () => {
  let component: SeatPlan;
  let fixture: ComponentFixture<SeatPlan>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeatPlan]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SeatPlan);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
