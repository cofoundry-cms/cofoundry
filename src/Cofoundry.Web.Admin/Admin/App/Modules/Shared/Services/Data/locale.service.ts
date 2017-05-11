import { Injectable }    from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { StringUtility } from '../../utilities/string.utility';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class LocaleService {
	private http: Http;
	private localeServiceBase = SERVICE_BASE + 'locales';

	constructor(http: Http) {
		this.http = http;
	}

	getAll() {
		return this.http
			.get(this.localeServiceBase);
	}
}
