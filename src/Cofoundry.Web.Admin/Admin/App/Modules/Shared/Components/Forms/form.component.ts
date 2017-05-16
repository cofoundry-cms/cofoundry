import { Component, Output, EventEmitter, ContentChildren, AfterContentInit, QueryList, ViewChild } from '@angular/core';
import { NgForm, NgModel } from '@angular/forms';
import { SELECTOR_PREFIX } from '../../constants/config.constants';
import { ElementBase } from './element-base';
import { FormActionSubmitComponent, FormActionEditComponent } from '../../../shared/components';

@Component({
	selector: SELECTOR_PREFIX + 'form',
	templateUrl: 'form.component.html',
	styleUrls: ['form.component.scss']
})

export default class Form implements AfterContentInit {
	@Output('cfSubmit') submit = new EventEmitter<NgForm>();
	@ContentChildren(ElementBase, { descendants: true }) public elements: QueryList<ElementBase<any>>;
	@ContentChildren(NgModel, { descendants: true }) public models: QueryList<NgModel>;
	@ContentChildren(FormActionSubmitComponent, { descendants: true }) public submitComponent: QueryList<FormActionSubmitComponent>;
	@ContentChildren(FormActionEditComponent, { descendants: true }) public editModeComponent: QueryList<FormActionEditComponent>;
	@ViewChild(NgForm) public form: NgForm;

	ngAfterContentInit() {
		let models = this.models.toArray();
		models.forEach((model) => {
			this.form.addControl(model);
		});

		let elements = this.elements.toArray();
		elements.forEach((el) => {
			el.editmode = false;
		});

		// Listen to form status changes. If we have a submit button
		// set its disable property
		this.listenToFormStatus(this.form, this.submitComponent);

		// Listen to editmode toggle
		this.listenToEditModeToggle(this.editModeComponent);
	}

	listenToFormStatus(form: NgForm, submitComponent: QueryList<FormActionSubmitComponent>): void {
		if (submitComponent.length > 0) {
			submitComponent.first.disabled = true;
			form.statusChanges.subscribe(status => {
				submitComponent.first.disabled = (status === 'INVALID');
			});
		}
	}

	listenToEditModeToggle(editModeComponent: QueryList<FormActionEditComponent>): void {
		if (editModeComponent.length > 0) {
			editModeComponent.first.editModeToggle.subscribe(editMode => {
				let elements = this.elements.toArray();
				elements.forEach((el) => {
					el.editmode = editMode;
				});
			});
		}
	}

	onSubmit(form) {
		this.submit.emit(form);
	}
}
