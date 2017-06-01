angular.module('cms.directories').factory('directories.DirectoryTree', [
    '_',
function (
    _) {

    return DirectoryTree;

    /**
     * Represents a query for searching entities and returning a paged result, handling
     * the persistance of the query parameters in the query string.
     */
    function DirectoryTree(originalTree) {
        var me = this;

        _.extend(me, originalTree);

        /* Public Funcs */

        /**
         * Flattens the node tree into a single array of nodes, optionally
         * excluding the directory with the specified id.
         */
        me.flatten = function (webDirectoryIdToExclude) {
            var allNodes = [];

            flattenNode(me, allNodes)

            return allNodes;

            function flattenNode(node, allNodes) {
                if (node.webDirectoryId == webDirectoryIdToExclude) return;
                allNodes.push(node);

                _.each(node.childWebDirectories, function (node) {
                    flattenNode(node, allNodes);
                });
            }
        }
        
        /**
         * Finds a directory node, searching through child nodes recursively.
         */
        me.findNodeById = function (webDirectoryIdToFind) {
            return findDirectory([me]);

            function findDirectory(directories) {
                var result;

                if (!directories) return;

                directories.forEach(function (directory) {
                    if (result) return;

                    if (directory.webDirectoryId == webDirectoryIdToFind) {
                        result = directory;
                    } else {
                        result = findDirectory(directory.childWebDirectories);
                    }

                });

                return result;
            }
        }
    }
}]);