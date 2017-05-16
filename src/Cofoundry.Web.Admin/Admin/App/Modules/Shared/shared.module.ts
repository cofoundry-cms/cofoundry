// Angular
import {
	NgModule,
	ModuleWithProviders }      		from '@angular/core';
import { RouterModule } 			from '@angular/router';
import { CommonModule }             from '@angular/common';
import { FormsModule }              from '@angular/forms';

// Pipes
import { BytesPipe }                from './pipes';

// Components
import {
	Button,
	ButtonLink,
	ButtonSubmit,
	FormActionEditComponent,
	FormActionSubmitComponent,
	Form,
	Fieldset,
	ValidationSummaryComponent,
	FormDynamicFieldSetComponent,
	FormFieldTextboxComponent,
	FormFieldSelectComponent,
	FormFieldSelectDirectoryComponent,
	FormFieldSelectLocaleComponent,
	ModalContainerComponent,
	PageHeader,
	PageActions,
	PageBody,
	PageRow, }  					from './components';
	

// Services
import {
	CustomEntityService,
	DocumentService,
	DirectoryService,
	EntityVersionService,
	HttpClient,
	ImageService,
	LocaleService,
	VimeoService,
	YoutubeService,
	ModalService,
	AuthenticationService,
	PermissionValidationService,
	ValidationErrorService }        from './services';

// Utils
import { StringUtility }            from './utilities/string.utility';

@NgModule({
	imports:      [ CommonModule, RouterModule, FormsModule ],
	declarations: [
		BytesPipe,
		PageHeader,
		PageActions,
		PageBody,
		PageRow,
		ModalContainerComponent,
		Button,
		ButtonLink,
		ButtonSubmit,
		FormFieldTextboxComponent,
		FormFieldSelectComponent,
		FormFieldSelectDirectoryComponent,
		FormFieldSelectLocaleComponent,
		FormDynamicFieldSetComponent,
		ValidationSummaryComponent,
		Fieldset,
		Form,
		FormActionEditComponent,
		FormActionSubmitComponent
	],
	exports:      [
		BytesPipe,
		PageHeader,
		PageActions,
		PageBody,
		PageRow,
		ModalContainerComponent,
		Button,
		ButtonLink,
		ButtonSubmit,
		FormFieldTextboxComponent,
		FormFieldSelectComponent,
		FormFieldSelectDirectoryComponent,
		FormFieldSelectLocaleComponent,
		FormDynamicFieldSetComponent,
		ValidationSummaryComponent,
		Fieldset,
		Form,
		FormActionEditComponent,
		FormActionSubmitComponent,
		CommonModule,
		FormsModule
	]
})
export class SharedModule {
	static forRoot(): ModuleWithProviders {
		return {
			ngModule: SharedModule,
			providers: [ StringUtility, ModalService ]
		};
	}
}

@NgModule({
	exports:   [ SharedModule ],
	providers: [ StringUtility, ModalService ]
})
export class SharedRootModule { }
