import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';
import { UrlLibrary } from '../../../shared/utilities/url-library.utility';

@Component({
	selector: SELECTOR_PREFIX + 'custom-entity-link',
	templateUrl: 'custom-entity-link.component.html',
	styleUrls: ['Custom-entities.component.scss'],
	providers: [
		UrlLibrary
	],
	inputs: [
		'customEntity',
		'customEntityDefinition'
	]
})

export class CustomEntityLink {
	customEntity: string = '=cmsCustomEntity';
	customEntityDefinition: string = '=cmsCustomEntityDefinition';

	constructor(
		private urlLibrary: UrlLibrary) {
		this.urlLibrary = urlLibrary;
	}
}
