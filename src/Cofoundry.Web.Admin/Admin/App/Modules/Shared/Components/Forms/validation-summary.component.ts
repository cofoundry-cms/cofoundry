import {Component, Input} from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: `${SELECTOR_PREFIX}validation-summary`,
	template: `
		<div class="alert alert-danger" role="alert">
			<span  *ngFor="let message of messages">
				{{message}}
			</span>
		</div>
	`,
	styles: [`
		.validation {
			color: #999;
			margin: 12px;
		}`
	]
})
export default class ValidationSummaryComponent {
	@Input() messages: Array<string>;
}
