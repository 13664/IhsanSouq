import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CharityCaseDetailsComponent } from './charity-case-details.component';

describe('CharityCaseDetailsComponent', () => {
  let component: CharityCaseDetailsComponent;
  let fixture: ComponentFixture<CharityCaseDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CharityCaseDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CharityCaseDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
