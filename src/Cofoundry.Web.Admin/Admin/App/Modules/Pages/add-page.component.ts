import { Component, OnInit, ViewChild } from '@angular/core';
import { PageService } from './page.service';
import { PageTemplateService } from './page-template.service';
import { CustomEntityService } from '../shared/services';
import { SELECTOR_PREFIX } from '../shared/constants/config.constants';
import { Form } from '../shared/components';

@Component({
	selector: `${SELECTOR_PREFIX}pages`,
	templateUrl: 'add-page.component.html',
	providers: [PageService, PageTemplateService, CustomEntityService]
})
export class AddPageComponent implements OnInit {
	@ViewChild(Form) form: Form;
	pageTypes: any[];
	pageTemplates: any[];
	routingRules: any[];

	constructor(
		private _pageService: PageService,
		private _pageTemplateService: PageTemplateService,
		private _customEntityService: CustomEntityService) { }

	ngOnInit() {
		this.pageTypes = this._pageService.getPageTypes();

		this._pageTemplateService
			.getAll()
			.subscribe(pageTemplates => {
				this.pageTemplates = pageTemplates.json().data.items;
			});

		this._customEntityService
			.getAllRoutingRules()
			.subscribe(routingRules => {
				this.routingRules = routingRules.json().data;
			});

		this.form.submit.subscribe(result => {
			//console.log(result);
		});
	}

	onSubmit(form) {
		console.log(form.value);
	}
}
