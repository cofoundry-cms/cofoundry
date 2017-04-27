import { Injectable } from '@angular/core';

@Injectable()
export default class AuthenticationService {
    redirectToLogin() {
        let loc = window.location;
        let path = `/admin/auth/login?returnUrl=${encodeURIComponent(loc.pathname + loc.hash)}`;
        //window.location = path;
    }
}