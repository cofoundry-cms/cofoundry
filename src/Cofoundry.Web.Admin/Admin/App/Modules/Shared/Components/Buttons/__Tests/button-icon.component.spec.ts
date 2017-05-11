import { TestBed } from '@angular/core/testing';
import { ButtonIcon } from '../button-icon.component';

describe('ButtonIcon', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({ declarations: [ButtonIcon]});
	});

	it ('should work', () => {
		TestBed.compileComponents().then(() => {
			let fixture = TestBed.createComponent(ButtonIcon);
			expect(fixture.componentInstance instanceof ButtonIcon).toBe(true, 'should create ButtonIcon');
		});
	});
});
