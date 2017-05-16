import { Component, Input } from '@angular/core';
import { FormGroup, FormControl, Validators }        from '@angular/forms';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: `${SELECTOR_PREFIX}form-dynamic-fieldset`,
	template: '<ng-content></ng-content>'
})
export default class FormDynamicFieldSetComponent {

}
