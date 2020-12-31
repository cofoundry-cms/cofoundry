angular.module('cms.shared').factory('shared.imageFileUtilities', [
    '$q',
function (
    $q
) {

    var service = {};

    /* PUBLIC */

    /**
     * Loads an image file, resizing the file is neccessary to meet
     * the maximum width/height requirements.
     */
    service.getFileInfoAndResizeIfRequired = function (file, settings) {

        return readFileIntoImgElement(file).then(onImageLoaded);

        function onImageLoaded(img) {

            if (!img) return null;

            var result = {
                fileName: getFileNameWithoutExtension(file.name),
                file: file,
                originalImageWidth: img.width,
                originalImageHeight: img.height,
                width: img.width,
                height: img.height,
                isResized: false
            };

            if ((settings.maxUploadWidth && img.width > settings.maxUploadWidth)
                || (settings.maxUploadHeight && img.height > settings.maxUploadHeight)) {
                // Note: png is only supported file type in spec, but others are typically supported.
                // Jpg is by far better at compressing images than the png encoder so let's use that.
                var fileType = 'image/jpeg';//file.type;

                var resizeResult = resizeFileDataAsBase64(img, file, fileType, settings);
                var u8Image = b64ToUint8Array(resizeResult.dataUrl);

                var formData = new FormData();
                formData.append('file', new Blob([u8Image], { type: fileType }), result.fileName + '.jpg');
                result.file = formData.get('file');
                result.isResized = true;
                result.width = resizeResult.width;
                result.height = resizeResult.height;
            }

            return result;
        }
    };

    /* PRIVATE HELPERS */

    function getFileNameWithoutExtension(filename) {
        return filename.substr(0, filename.lastIndexOf('.')) || filename;
    }

    function readFileIntoImgElement(file) {
        var def = $q.defer();

        if (!file) {
            def.resolve(null);
        }
        else {
            var reader = new FileReader();
            reader.onload = onReaderLoad;
            reader.onerror = onError;
            reader.readAsDataURL(file);
        }

        return def.promise;

        function onReaderLoad(e) {

            var img = document.createElement("img");
            img.onload = onImageLoad;
            img.onerror = onError;
            img.src = e.target.result;

            // ensure file is loaded in the dom element before proceeding (intermittent Edge bug)
            function onImageLoad() {
                def.resolve(img);
            }
        }

        function onError(err) {
            def.resolve(null);
        }
    }

    function resizeFileDataAsBase64(img, file, fileType, settings) {

        if (!img || !file) return null;

        var canvas = document.createElement("canvas");
        var width = img.width;
        var height = img.height;

        if (!width || !height) {
            return null;
        }

        var widthRatio = getResizeRatio(width, settings.maxUploadWidth);
        var heightRatio = getResizeRatio(height, settings.maxUploadHeight);
        var ratioToUse = widthRatio < heightRatio ? widthRatio : heightRatio;

        width = Math.round(ratioToUse * width);
        height = Math.round(ratioToUse * height);

        canvas.width = width;
        canvas.height = height;

        var ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, width, height);

        var result = {
            dataUrl: canvas.toDataURL(fileType),
            width: width,
            height: height
        }

        return result;

        function getResizeRatio(size, max) {
            if (size > max) {
                return max / size;
            } else {
                return 1;
            }
        }
    }

    function b64ToUint8Array(b64Image) {
        var img = atob(b64Image.split(',')[1]);
        var img_buffer = [];
        var i = 0;
        while (i < img.length) {
            img_buffer.push(img.charCodeAt(i));
            i++;
        }
        return new Uint8Array(img_buffer);
    }
    
    return service;
}]);