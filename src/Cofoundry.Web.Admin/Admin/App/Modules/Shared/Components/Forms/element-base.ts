import { ViewChild } from '@angular/core';
import { NgModel, NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { ValueAccessorBase } from './value-accessor';
import {
	AsyncValidatorArray,
	ValidatorArray,
	ValidationResult,
	message,
	validate,
} from './validate';

export abstract class ElementBase<T> extends ValueAccessorBase<T> {
	protected abstract model: NgModel;
	@ViewChild('form') form;

  	// we will ultimately get these arguments from @Inject on the derived class
	constructor(private validators: ValidatorArray,
		private asyncValidators: AsyncValidatorArray,
	) {
		super();
		console.log(this.form);
	}

	protected validate(): Observable<ValidationResult> {
		return validate
			(this.validators, this.asyncValidators)
			(this.model.control);
	}

	protected get invalid(): Observable<boolean> {
		return this.validate().map(v => Object.keys(v || {}).length > 0);
	}

	protected get failures(): Observable<Array<string>> {
		return this.validate().map(v => Object.keys(v).map(k => message(v, k)));
	}
}
