import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { AiPipeline } from './ai-pipeline';

describe('AiPipeline', () => {
  let fixture: ComponentFixture<AiPipeline>;
  let component: AiPipeline;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AiPipeline, RouterTestingModule],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AiPipeline);
    component = fixture.componentInstance;
  });

  it('marks all earlier stages as done and the last stage as active when the request is completed', () => {
    component.request = {
      id: '1',
      patientName: 'Test Patient',
      requiredBloodType: 'A+',
      urgency: 'عاجل',
      status: 'مكتمل',
      unitsNeeded: 2,
      latitude: 30.0444,
      longitude: 31.2357,
    } as any;

    expect(component.isCompletedRequest()).toBeTrue();
    expect(component.getPipelineState(1)).toBe('done');
    expect(component.getPipelineState(5)).toBe('done');
    expect(component.getPipelineState(6)).toBe('active');
  });
});
