import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminBuses } from './admin-buses';

describe('AdminBuses', () => {
  let component: AdminBuses;
  let fixture: ComponentFixture<AdminBuses>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminBuses],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminBuses);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
