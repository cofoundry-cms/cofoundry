import { Component, Input } from '@angular/core';
import { BaseFormFieldComponent } from './base-form-field.component';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
  selector: SELECTOR_PREFIX + 'form-field-textbox',
  templateUrl: 'form-field-textbox.component.html',
  inputs: [
      'title',
      'type'
  ]
})
export default class FormFieldTextboxComponent extends BaseFormFieldComponent<string> {
    title: string;
    type: string;
    controlType = 'textbox';
    model: {};

    constructor() {
        super();
        //this.type = options['type'] || 'text';
    }
}