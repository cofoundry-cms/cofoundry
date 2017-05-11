import { Injectable }    from '@angular/core';
import { Http } from '@angular/http';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class DirectoryService {
	private http: Http;
	private directoryServiceBase = SERVICE_BASE + 'webdirectories';

	constructor(http: Http) {
		this.http = http;
	}

	getAll() {
		return this.http
			.get(this.directoryServiceBase);
	}
}
