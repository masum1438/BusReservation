import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminPassengers } from './admin-passengers';

describe('AdminPassengers', () => {
  let component: AdminPassengers;
  let fixture: ComponentFixture<AdminPassengers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminPassengers],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminPassengers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
