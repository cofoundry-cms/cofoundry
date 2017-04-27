// Angular
import { NgModule,
         ModuleWithProviders }      from '@angular/core';
import { CommonModule }             from '@angular/common';
import { FormsModule }              from '@angular/forms';

// Pipes
import { BytesPipe }                from './pipes';

// Components
import { ModalContainerComponent }  from './components'

// Services
import {
    CustomEntityService,
    DocumentService,
    EntityVersionService,
    HttpClient,
    ImageService,
    LocaleService,
    VimeoService,
    YoutubeService,
    ModalService,
    AuthenticationService,
    PermissionValidationService,
    ValidationErrorService }        from './services'

// Utils
import { StringUtility }            from './utilities/string.utility';

@NgModule({
  imports:      [ CommonModule ],
  declarations: [ BytesPipe ],
  exports:      [ BytesPipe,
                  CommonModule, FormsModule ]
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