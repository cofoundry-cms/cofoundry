import { Component, Input } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'fieldset',
	styleUrls: ['./fieldset.component.scss'],
    templateUrl: 'fieldset.component.html'
})

export default class Fieldset {
    @Input() public label: string;
}