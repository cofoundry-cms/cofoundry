import { TestBed } from '@angular/core/testing';
import { DashboardComponent } from '../dashboard.component';

describe('DashboardComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ declarations: [DashboardComponent]});
  });

  it ('should work', () => {
    TestBed.compileComponents().then(() => {
      let fixture = TestBed.createComponent(DashboardComponent);
      expect(fixture.componentInstance instanceof DashboardComponent).toBe(true, 'should create DashboardComponent');
    });
  });
});