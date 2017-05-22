import { NgModel } from '@angular/forms';
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
	public abstract model: NgModel;
	private editMode: boolean = false;

	constructor(private validators: ValidatorArray,
		private asyncValidators: AsyncValidatorArray,
	) {
		super();
	}

	protected validate(): Observable<ValidationResult> {
		if (!this.model) {
			return Observable.of(null);
		}

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

	public set editmode(state: boolean) {
		this.editMode = state;
	}
}
