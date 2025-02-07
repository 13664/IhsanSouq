import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CharityCaseItemComponent } from './charity-case-item.component';

describe('CharityCaseItemComponent', () => {
  let component: CharityCaseItemComponent;
  let fixture: ComponentFixture<CharityCaseItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CharityCaseItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CharityCaseItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
