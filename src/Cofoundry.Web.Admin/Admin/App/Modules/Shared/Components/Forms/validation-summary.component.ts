import {Component, Input} from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: `${SELECTOR_PREFIX}validation-summary`,
	template: `
		<div class="validation">
			<div *ngFor="let message of messages">{{message}}</div>
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
