/**
 * Condenses down a url path to maximum length, limiting directory
 * segments or omitting them completely if necessary. The last segment
 * of the path is treated as the main identifier and is always preserved in full, 
 * which may be longer than the max length. The aim is to keep the identifier
 * segment in readable view.
 */
angular.module('cms.shared').filter('urlPathCondenser', function () {
    return function limitUrlPath(path, maxLength, maxSegmentLength) {
        maxLength = maxLength || 60;
        maxSegmentLength = maxSegmentLength || 25;

        if (!path) return '';

        if (path.length < maxLength)
        {
            return path;
        }

        var segments = path.split('/');
        var condensedUrl = '';

        for (var i = segments.length -1; i >= 0; i--)
        {
            var segment = segments[i];
            if (!segment) continue;
            
            // if we can only fit one segment, return it unadorned
            var hasReachedMaxLength = condensedUrl.length + segment.length > maxLength;
            if (hasReachedMaxLength && !condensedUrl) {
                return segment;
            }

            // if the segement is too long, limit it
            if (segment.length > maxSegmentLength) {

                segment = segment.substring(0, maxSegmentLength -1) + '…';
            }

            // if we've reached the max-length even when condensed, return what we have
            hasReachedMaxLength = condensedUrl.length + segment.length > maxLength;
            if (hasReachedMaxLength) {
                return '..' + condensedUrl;
            }
            
            condensedUrl = '/' + segment + condensedUrl;
        }

        return condensedUrl;
    }
});