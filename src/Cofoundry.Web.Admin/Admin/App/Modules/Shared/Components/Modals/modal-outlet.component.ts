import { Component, ViewContainerRef, ViewChild } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';
import { ModalService } from '../../services';

@Component({
    selector: SELECTOR_PREFIX + 'modal-outlet',
    template: `
        <div #modalViewContainerRef></div>
        <!--<div class="modal-background large"></div>-->
    `
})
export class ModalOutletComponent {
    @ViewChild('modalViewContainerRef', { read: ViewContainerRef }) modalViewContainerRef;

    constructor(private modalService: ModalService) {}

    ngAfterViewInit() {
        this.modalService.viewContainerRef = this.modalViewContainerRef;
    }
}