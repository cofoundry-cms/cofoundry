import { Type, ComponentRef, Injectable, Component, Input, AfterViewInit, ViewChild,
	ComponentFactoryResolver, OnDestroy } from '@angular/core';
import { ModalOptionsArgs } from '../index';
import { ModalDirective } 	from '../../Components/Modals/modal.directive';
import { IModalComponent } 	from '../../Components/Modals/modal.component';

@Injectable()
export default class ModalService {
	@ViewChild(ModalDirective) cfModal: ModalDirective;

	// modals: ComponentRef<any>[] = [];

	constructor(
		private _componentFactoryResolver: ComponentFactoryResolver) {}

	public alert() {

	}

	public confirm() {

	}

	public open(args?: ModalOptionsArgs) {
		this.showModal(args);
	}

	public close() {
		// let modal = this.modals.pop();
		// modal.destroy();
	}

	private showModal(args?: ModalOptionsArgs) {
		let modal = new ModalItem(null, {});
		let componentFactory = this._componentFactoryResolver.resolveComponentFactory(modal.component);

		let viewContainerRef = this.cfModal.viewContainerRef;
		viewContainerRef.clear();

		let componentRef = viewContainerRef.createComponent(componentFactory);
		(<IModalComponent>componentRef.instance).data = modal.data;
	}
}

export class ModalItem {
	constructor(public component: Type<any>, public data: any) {}
}
