import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminShcedules } from './admin-shcedules';

describe('AdminShcedules', () => {
  let component: AdminShcedules;
  let fixture: ComponentFixture<AdminShcedules>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminShcedules],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminShcedules);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
