import { Injectable }    from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { StringUtility } from '../../utilities/string.utility';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class ImageService {
	private imagesServiceBase = SERVICE_BASE + 'images';

	constructor(private http: Http) {}

	getAll(query) {
		return this.http
			.get(this.imagesServiceBase, {
				search: query
			});
	}

	getById(imageId) {
		return this.http
			.get(this.getIdRoute(imageId));
	}

	getByIdRange(ids) {
		let searchParams = new URLSearchParams();
		searchParams.set('ids', ids);

		return this.http
			.get(this.imagesServiceBase + '/', {
				search: searchParams
			});
	}

	private getIdRoute(imageId) {
		return this.imagesServiceBase + '/' + imageId;
	}

	private getBaseRoute() {
		return this.imagesServiceBase;
	}
}
