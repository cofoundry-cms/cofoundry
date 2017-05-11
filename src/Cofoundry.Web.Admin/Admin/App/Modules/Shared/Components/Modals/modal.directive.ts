import { Directive, ViewContainerRef } from '@angular/core';

@Directive({
	selector: '[cf-modal]'
})
export class ModalDirective {
	constructor(public viewContainerRef: ViewContainerRef) { }
}
