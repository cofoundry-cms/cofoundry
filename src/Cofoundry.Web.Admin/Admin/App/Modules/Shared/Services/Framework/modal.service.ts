import { NgModuleFactoryLoader, Injectable, Component, ComponentFactoryResolver, 
    ViewContainerRef, ComponentFactory, ReflectiveInjector, ComponentRef } from '@angular/core';
import { ModalOptionsArgs } from '../index';

@Injectable()
export default class ModalService {
    viewContainerRef;
    modals:ComponentRef<any>[] = [];

    constructor(
        private componentResolver: ComponentFactoryResolver) {}

    public alert() {

    }

    public confirm() {

    }

    public open(component, args?: ModalOptionsArgs) {
        this.showModal(component, args);
    }

    public close() {
        let modal = this.modals.pop();
        modal.destroy();
    }

    private showModal(component, args?: ModalOptionsArgs) {
        let factory = this.createComponentFactory(this.componentResolver, component);
        console.log(factory);

            //.then(factory => {
                //const injector = ReflectiveInjector.fromResolvedProviders([], this.viewContainerRef.injector);
                //this.modals.push(this.viewContainerRef.createComponent(factory, 0, injector, []));
            //});
    }

    private createComponentFactory(resolver: ComponentFactoryResolver, component): ComponentFactory<any> {
        return resolver.resolveComponentFactory(component);
    }
}
