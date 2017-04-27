import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../constants/config.constants';
import { UrlLibrary } from '../../shared/utilities/url-library.utility';
import { ModalService } from '../../shared/services';

@Component({
    selector: SELECTOR_PREFIX + 'temp-component',
    template: `
        <div class="modal large modal--show">
            <div class="modal-dialog">
                <div class="modal-content">
                    <button type="button" (click)="openAnother()">Open another modal</button>
                    <button type="button" (click)="close()">Close</button>
                </div>
            </div>
        </div>
    `
})
export class TempComponent {
    title = `Temp component ${Date.now()}`;

    constructor(private modalService: ModalService) {}

    openAnother() {
        this.modalService.open(TempComponent);
    }

    close() {
        this.modalService.close();
    }
}