# Cofoundry.DocGenerator

The doc generator tool is used to copy, format and index doc files from the Cofoundry documentation repository into the cofoundry.org site. Key features are:

- Copying files and standardizing file names to urls friendly values.
- Markdown files are copied as-is into the same doc structure
- Static resources are copied into a separate, public servable directory.
- Files are copied to a folder based on the version of the docs being published (e.g. /0.5.0/)
- Versions are indexed into a versions.json file in the root
- The documentation file structure is indexed into a toc.json file which includes titles, paths and other information
- Any redirects are parsed and inserted into the doc

For more information on the structure of the documentation see the main [docs README](/docs/README.md).
