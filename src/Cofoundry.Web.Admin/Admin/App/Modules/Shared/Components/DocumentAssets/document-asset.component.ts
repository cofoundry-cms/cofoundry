import { Component, OnInit } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';
import { UrlLibrary } from '../../../shared/utilities/url-library.utility';

@Component({
    selector: SELECTOR_PREFIX + 'document-asset',
    templateUrl: 'document-asset.component.html',
    styleUrls: ['Document-assets.component.scss'],
    inputs: [
        'document'
    ]
})

export class DocumentAsset implements OnInit {
    document;
    getDocumentUrl;

    constructor(private urlLibrary: UrlLibrary) {}

    ngOnInit() {
        this.getDocumentUrl = this.urlLibrary.getDocumentUrl; 
    }
}