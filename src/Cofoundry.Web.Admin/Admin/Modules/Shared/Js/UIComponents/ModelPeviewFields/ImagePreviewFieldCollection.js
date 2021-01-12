/**
 * Helper used for working with collections of dynamic model data that
 * might use the [PreviewImage] data annotation to provide an image preview
 * field. This helper extracts the ids, loads the data and provides methods
 * for upading the dataset without having to reload all the images.
 */
angular.module('cms.shared').factory('shared.ImagePreviewFieldCollection', [
    '$q',
    '_',
    'shared.arrayUtilities',
    'shared.imageService',
    function (
    $q,
    _,
    arrayUtilities,
    imageService
) {
        return ImagePreviewFieldCollection;

        function ImagePreviewFieldCollection(partitionByProperty) {
            var me = this,
                imagePropertyName,
                cachedFieldSet,
                PREVIEW_IMAGE_FIELD_NAME = 'previewImage';

            /* Public Properties */

            //me.images

            /* Public Funcs */

            me.load = function (dataset, fieldSet) {
                cachedFieldSet = fieldSet;

                if (!dataset
                    || !dataset.length
                    || !fieldSet) return resolveNoData();

                var allImageIds = _.chain(dataset)
                    .map(function (item) {
                        return modelPropertyAccessor(item, getImagePropertyName(item));
                    })
                    .filter(function (id) {
                        return !!id;
                    })
                    .uniq()
                    .value();

                if (!allImageIds.length) return resolveNoData();

                return imageService.getByIdRange(allImageIds).then(function (images) {
                    me.images = [];

                    _.each(dataset, function (item) {
                        var id = modelPropertyAccessor(item, getImagePropertyName(item)),
                            image;

                        if (id) {
                            image = _.find(images, { imageAssetId: id });
                        }

                        me.images.push(image);
                    });
                });

                function resolveNoData() {
                    var deferred = $q.defer();
                    deferred.resolve();

                    me.images = [];

                    return deferred.promise;
                }
            };

            me.move = function (itemToMoveIndex, moveToIndex) {
                arrayUtilities.move(me.images, itemToMoveIndex, moveToIndex);
            };

            me.add = function (itemToAdd, index) {
                return updateImage(itemToAdd, index, true);
            };

            me.update = function (itemToUpdate, index) {
                return updateImage(itemToUpdate, index);
            };

            me.remove = function (index) {
                arrayUtilities.remove(me.images, index);
            };

            /* Private */

            function getImagePropertyName(model) {
                // In the case of multi-type grids we can partition fieldsets by a property
                if (partitionByProperty) {
                    var partitionId = model[partitionByProperty];
                    var field = cachedFieldSet[partitionId].fields[PREVIEW_IMAGE_FIELD_NAME];

                    if (!field) return undefined;

                    return field.lowerName;
                }

                // cache the property name
                if (!imagePropertyName && cachedFieldSet.fields[PREVIEW_IMAGE_FIELD_NAME]) {
                    imagePropertyName = cachedFieldSet.fields[PREVIEW_IMAGE_FIELD_NAME].lowerName;
                }

                return imagePropertyName;
            }

            function updateImage(itemToUpdate, index, isNew) {
                var propertyName = getImagePropertyName(itemToUpdate);
                if (!propertyName) return;

                var newImageId = modelPropertyAccessor(itemToUpdate, propertyName);

                if (!isNew) {
                    var existingImage = me.images[index],
                        existingId;

                    if (existingImage) {
                        existingId = existingImage['imageAssetId'];
                    }

                    if (newImageId == existingId) return;

                    if (!newImageId) {
                        me.images[index] = undefined;
                        return;
                    }
                }

                return imageService
                    .getById(newImageId)
                    .then(loadImage);

                function loadImage(image) {
                    me.images[index] = image;
                }
            }

            function modelPropertyAccessor(item, propertyName) {

                if (!propertyName) return undefined;

                // if the model is a child of the item e.g. custom entities
                if (item.model) return item.model[propertyName];

                return item[propertyName];
            }
        }
}]);