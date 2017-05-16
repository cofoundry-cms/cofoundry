import { Component, Input } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';
import './page-header.component.scss';

@Component({
	selector: SELECTOR_PREFIX + 'page-header',
	styleUrls: ['./page-header.component.scss'],
	templateUrl: 'page-header.component.html'
})

export default class PageHeader {
	@Input() title;
	@Input('parent-title') parentTitle;
}
