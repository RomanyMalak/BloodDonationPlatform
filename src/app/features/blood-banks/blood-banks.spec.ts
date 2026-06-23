import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BloodBanks } from './blood-banks';

describe('BloodBanks', () => {
  let component: BloodBanks;
  let fixture: ComponentFixture<BloodBanks>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BloodBanks],
    }).compileComponents();

    fixture = TestBed.createComponent(BloodBanks);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
