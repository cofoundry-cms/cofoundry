angular.module('cms.shared').factory('shared.ModelPreviewFieldset', [
    '$q',
    '_',
    'shared.stringUtilities',
    'shared.imageService',
function (
    $q,
    _,
    stringUtilities,
    imageService
) {
    return ModelPreviewFieldset;

    function ModelPreviewFieldset(modelMetaData) {
        var me = this,
            PREVIEW_TITLE_FIELD_NAME = 'previewTitle',
            PREVIEW_DESCRIPTION_FIELD_NAME = 'previewDescription',
            PREVIEW_IMAGE_FIELD_NAME = 'previewImage';

        /* Public Properties */

        me.modelMetaData = modelMetaData;
        me.fields = parseFields(modelMetaData);
        me.showTitle = canShowTitleColumn(me.fields);
        me.titleTerm = getTitleTerm(me.fields);

        /* Public Funcs */

        me.on = function () {
            me.isLoading = true;
            if (me.progress === 100) {
                me.progress = 0;
            }
        };

        /* Private */

        function parseFields(modelMetaData) {
            var fields = {};

            setGridField(PREVIEW_TITLE_FIELD_NAME);
            setGridField(PREVIEW_DESCRIPTION_FIELD_NAME);
            setGridField(PREVIEW_IMAGE_FIELD_NAME);

            return fields;

            function setGridField(fieldName) {

                var field = _.find(modelMetaData.dataModelProperties, function (property) {

                    return property.additionalAttributes[fieldName];
                });

                if (field) {
                    field.lowerName = stringUtilities.lowerCaseFirstWord(field.name);
                    fields[fieldName] = field;
                    fields.hasFields = true;
                }
            }
        }

        /**
         * Title is shown when a [PreviewTitle] attribute is present 
         * or if no other preview attributes are present.
         */
        function canShowTitleColumn(gridFields) {
            return gridFields[PREVIEW_TITLE_FIELD_NAME] || !gridFields.hasFields;
        }

        /**
         * The title field will default to "Title" but optionally
         * a different field can be specified as the title using 
         * the [PreviewTitle] attribute
         */
        function getTitleTerm(gridFields) {

            if (gridFields[PREVIEW_TITLE_FIELD_NAME]) {
                return gridFields[PREVIEW_TITLE_FIELD_NAME].displayName;
            }

            return "Title";
        }

        function loadImageFields() {
            if (!vm.result || !vm.gridFields || !vm.gridFields[PREVIEW_IMAGE_FIELD_NAME]) return;

            var field = vm.gridFields[PREVIEW_IMAGE_FIELD_NAME];

            var allImageIds = _.chain(vm.result.items)
                .map(function (item) {
                    return item.model[field.lowerName];
                })
                .filter(function (id) {
                    return id;
                })
                .uniq()
                .value();

            return imageService.getByIdRange(allImageIds).then(function (images) {
                vm.modelImages = [];

                _.each(vm.result.items, function (item) {
                    var id = item.model[field.lowerName],
                        image;

                    if (id) {
                        image = _.find(images, { imageAssetId: id });
                    }

                    vm.modelImages.push(image);
                });
            });
        }

    }
}]);