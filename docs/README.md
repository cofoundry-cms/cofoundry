# Cofoundry User Docs

Source for documentation on [cofoundry.org](https://cofoundry.org/docs) is in the [docs/user-docs](/docs/user-docs) folder. The user docs are a standard markdown wiki with some enhanced features:

- Version support (based on tags) with canonical urls
- Nested folder support
- Ability to define redirects
- Folder level TOC (table of contents) support, with default ordering if not specified

## File Structure

The documentation follows a few structural rules:

### Directory and file naming

- The directory and file names are used as-is as the document/directory title
- Directory and file names are slugged to created the url paths
- Directories can be nested to create nested navigation structures 

### Directory root documents

- Add an *index.md* to customize the directory root-level documentation
- If no index.md file is specified then a list of child documents will be displayed automatically

### Links

- Links will be transformed as part of the markdown rendering on the cofoundry.org site
- To reference another document, just use the title or slug - we will convert it to a slug either way
- You can reference documents in the same directory or child directory by not using a slash at the start e.g. `[My Doc](My Doc)` or `[My Doc](My Folder/My Doc)`
- Parent directory traversal with '../' is not supported yet
- To reference files outside of your directory you can use an application relative root eg. `[My Doc](/My Folder/My Doc)`


### Static resources

- Any files except *.md* and *.json* files are copied to a separate directory so they can be hosted as static resources
- Static files can be nested in directories and will retain the same structure when copied to the output directory
- File names are slugified when copied
- References to static files are re-written to the correct directory/file name

## Table Of Contents

- By default a table of contents *(TOC)* will be created using the file and directory structure, ordering items by the document name.
- The included files and ordering can be customized by defining a *toc.json* file with an array of document titles you want to include in the order you want to include them e.g:

```json
[
	"Website Startup",
	"Data Access",
	"Mail",
	"Background Tasks"
]
```

## Redirects

If documents get moved or renamed you should create a redirect. To do this first create a *redirects.json* file in the folder with the file you want to redirect. The json file should contain an object with keys mapping to a file name and the value mapping to the redirected path. This translates to a .NET `Dictionary<string,string>`, e.g:

```json
{
    "Website Startup": "/new-folder/website-startup",
    "Mail": "/mail"
}
```

## Building the docs

The docs are published using the [Doc Generator tool](/eng/Cofoundry.DocGenerator/README.md) as part of the build process.