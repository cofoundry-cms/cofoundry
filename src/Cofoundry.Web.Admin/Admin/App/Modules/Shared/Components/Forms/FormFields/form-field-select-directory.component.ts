import { Component, Input, ViewChild, Optional, OnInit, Inject, forwardRef } from '@angular/core';
import { SELECTOR_PREFIX } from '../../../constants/config.constants';
import {
	NgModel,
	NG_VALUE_ACCESSOR,
	NG_VALIDATORS,
	NG_ASYNC_VALIDATORS,
} from '@angular/forms';
import { ElementBase } from '../element-base';
import { DirectoryService } from '../../../services';

@Component({
	selector: `${SELECTOR_PREFIX}form-field-select-directory`,
	templateUrl: 'form-field-select-directory.component.html',
	providers: [
		DirectoryService,
		{ provide: NG_VALUE_ACCESSOR, useExisting: FormFieldSelectDirectoryComponent, multi: true },
		{ provide: ElementBase, useExisting: forwardRef(() => FormFieldSelectDirectoryComponent) }
	]
})
export default class FormFieldSelectDirectoryComponent extends ElementBase<string> implements OnInit {
	@Input() public label: string;
	@Input() public placeholder: string;
	@ViewChild(NgModel) model: NgModel;
	directories: any[];

	constructor(
		@Optional() @Inject(NG_VALIDATORS) validators: Array<any>,
		@Optional() @Inject(NG_ASYNC_VALIDATORS) asyncValidators: Array<any>,
		private _directoryService: DirectoryService
	) {
		super(validators, asyncValidators);
	}

	ngOnInit() {
		this._directoryService
			.getAll()
			.subscribe(response => {
				this.directories = response.json().data;
			});
	}
}
