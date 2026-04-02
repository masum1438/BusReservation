import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminRoutes } from './admin-routes';

describe('AdminRoutes', () => {
  let component: AdminRoutes;
  let fixture: ComponentFixture<AdminRoutes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminRoutes],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminRoutes);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
