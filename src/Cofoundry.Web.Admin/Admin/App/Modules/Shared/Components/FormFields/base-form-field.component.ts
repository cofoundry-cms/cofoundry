import { Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

export class BaseFormFieldComponent<T> {
    @Input() form: FormGroup;
    value: T;
    key: string;
    label: string;
    required: boolean;
    order: number;
    controlType: string;

    constructor(options: {
        value?: T,
        key?: string,
        label?: string,
        required?: boolean,
        order?: number,
        controlType?: string
    } = {}) {
        this.value = options.value;
        this.key = options.key || '';
        this.label = options.label || '';
        this.required = !!options.required;
        this.order = options.order === undefined ? 1 : options.order;
        this.controlType = options.controlType || '';
    }

    get isValid() { 
        return this.form.controls[this.key].valid; 
    }
}