import { Component, EventEmitter, Output } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'form-action-edit',
	template: `<input class="btn main-cta" type="button" value="{{title}}" (click)="onClick()" />`,
	inputs: [
		'title',
		'disabled'
	]
})

export default class FormActionEditComponent {
	@Output() editModeToggle: EventEmitter<boolean> = new EventEmitter<boolean>();
	private editMode: boolean = false;
	title: string;

	onClick() {
		this.editMode = !this.editMode;
		this.editModeToggle.emit(this.editMode);
	}
}
