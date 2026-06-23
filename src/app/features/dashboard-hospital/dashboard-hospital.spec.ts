import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardHospital } from './dashboard-hospital';

describe('DashboardHospital', () => {
  let component: DashboardHospital;
  let fixture: ComponentFixture<DashboardHospital>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardHospital],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardHospital);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
