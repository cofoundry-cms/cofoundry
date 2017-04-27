import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'form',
    templateUrl: 'form.component.html',
    //styleUrls: ['form.component.scss']
})

export default class Form {
    onSubmit(event) {
        console.log(event);
    }
}
