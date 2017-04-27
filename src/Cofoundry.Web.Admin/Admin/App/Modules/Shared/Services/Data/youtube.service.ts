import { Injectable }    from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { StringUtility } from '../../utilities/string.utility';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class YoutubeService {
    private http: Http;
    private serviceKey = 'AIzaSyA1lW3d0K_SxwgQsYXGIXANhMwa013nZXg';
    private serviceUrl = 'https://www.googleapis.com/youtube/v3/videos?id=';

    constructor(http: Http) {
        this.http = http;
    }

    getVideoInfo(id) {
        return this.http
            .get(`${this.serviceUrl}${id}&part=contentDetails&key=${this.serviceKey}`)
            //.map(res => res.json())
            .subscribe(
                //res => res.data[0], 
                //err => {}
            );
    }
}