import {
	AbstractControl,
	AsyncValidatorFn,
	Validator,
	Validators,
	ValidatorFn,
} from '@angular/forms';

import {Observable} from 'rxjs';

export type ValidationResult = {[validator: string]: string | boolean};

export type AsyncValidatorArray = Array<Validator | AsyncValidatorFn>;

export type ValidatorArray = Array<Validator | ValidatorFn>;

const normalizeValidator =
	(validator: Validator | ValidatorFn): ValidatorFn | AsyncValidatorFn => {
	const func = (validator as Validator).validate.bind(validator);
	if (typeof func === 'function') {
		return (c: AbstractControl) => func(c);
	} else {
		return <ValidatorFn | AsyncValidatorFn> validator;
	}
};

export const composeValidators =
	(validators: ValidatorArray): AsyncValidatorFn | ValidatorFn => {
	if (validators == null || validators.length === 0) {
		return null;
	}
	return Validators.compose(validators.map(normalizeValidator));
};

export const validate =
	(validators: ValidatorArray, asyncValidators: AsyncValidatorArray) => {
	return (control: AbstractControl) => {
		if (!control.touched) {
			return Observable.of(null);
		}

		const synchronousValid = () => composeValidators(validators)(control);

		if (asyncValidators) {
			const asyncValidator = composeValidators(asyncValidators);

			return asyncValidator(control).map(v => {
				const secondary = synchronousValid();
				if (secondary || v) { // compose async and sync validator results
					return Object.assign({}, secondary, v);
				}
			});
		}

		if (validators) {
			return Observable.of(synchronousValid());
		}

		return Observable.of(null);
	};
};

export const message = (validator: ValidationResult, key: string): string => {
	switch (key) {
		case 'required':
			return 'Please enter a value';
		case 'pattern':
			return 'Value does not match required pattern';
		case 'minlength':
			return 'Value must be N characters';
		case 'maxlength':
			return 'Value must be a maximum of N characters';
	}

	switch (typeof validator[key]) {
		case 'string':
			return <string> validator[key];
		default:
			return `Validation failed: ${key}`;
	}
};
