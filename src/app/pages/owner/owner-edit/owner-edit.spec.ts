import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OwnerEdit } from './owner-edit';

describe('OwnerEdit', () => {
  let component: OwnerEdit;
  let fixture: ComponentFixture<OwnerEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OwnerEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OwnerEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
