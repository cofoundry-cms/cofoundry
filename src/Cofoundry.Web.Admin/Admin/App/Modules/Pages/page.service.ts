import { Injectable }    from '@angular/core';
import { Headers, Http, RequestOptionsArgs } from '@angular/http';
import { SERVICE_BASE } from '../shared/constants/path.constants';

@Injectable()
export class PageService {
	pagesServiceBase = SERVICE_BASE + 'pages';

	constructor(private http: Http) {}

	/* QUERIES */

	getAll(query) {
		let reqOptions: RequestOptionsArgs = {
			search: query
		};

		return this.http
			.get(this.pagesServiceBase, reqOptions);
	}

	getById(pageId) {
		return this.http
			.get(this.getIdRoute(pageId));
	}

	getVersionsByPageId(pageId) {
		return this.http
			.get(this.getPageVerionsRoute(pageId));
	}

	getPageTypes = function () {
		return [{
			name: 'Generic',
			value: 'Generic'
		},
		{
			name: 'Custom Entity Details',
			value: 'CustomEntityDetails'
		},
		{
			name: 'Not Found',
			value: 'NotFound'
		}];
	}

	/* COMMANDS */

	add(command) {
		return this.http
			.post(this.pagesServiceBase, command);
	}

	update(command) {
		return this.http
			.patch(this.getIdRoute(command.pageId), command);
	}

	updateUrl(command) {
		return this.http
			.put(this.getIdRoute(command.pageId) + '/url', command);
	}

	updateDraft(command) {
		return this.http
			.patch(this.getPageVerionsRoute(command.pageId) + '/draft', command);
	}

	removeDraft(id) {
		return this.http
			.delete(this.getPageVerionsRoute(id) + '/draft');
	}

	remove(id) {
		return this.http
			.delete(this.getIdRoute(id));
	}

	duplicate(command) {
		return this.http
			.post(this.getIdRoute(command.pageToDuplicateId) + '/duplicate', command);
	}

	/* PRIVATES */

	/* HELPERS */

	getIdRoute(pageId) {
		return this.pagesServiceBase + '/' + pageId;
	}

	getPageVerionsRoute(pageId) {
		return this.getIdRoute(pageId) + '/versions';
	}
}
