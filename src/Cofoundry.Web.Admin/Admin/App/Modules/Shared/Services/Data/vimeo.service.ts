import { Injectable }    from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { StringUtility } from '../../utilities/string.utility';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class VimeoService {
    private http: Http;
    private serviceUrl = '//vimeo.com/api/v2/video/';

    constructor(http: Http) {
        this.http = http;
    }

    getVideoInfo(id) {
        return this.http
            .get(`${this.serviceUrl}${id}.json`)
            //.map(res => res.json())
            .subscribe(
                //res => res.data[0], 
                //err => {}
            );
    }
}
