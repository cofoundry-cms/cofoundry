# Cofoundry Sass structure

This document should outline and explain the structure and methods used for the styling of the Cofoundry Admin site. 

*This document is a work in progress*

## Module structure
### Architecture
Each Module has a Sass folder, which compiles down to a single CSS file for that module. This can be controlled either by a global Build process located in the root of the **Project**, or from a local Build process located in the root of the **Module**. 

The structure looks like this:
```
Cofoundry.Web.Admin
|-- Build
    |-- gruntfile.js
    |-- package.json
|-- Admin
    |-- Modules
        |-- Shared
            |-- config.rb
            |-- Build
                |-- gruntfile.js
                |-- package.json
            |-- Sass
                |-- shared.scss
                |-- config
                |-- common-base
                |-- global
                |-- libs
                |-- third-party
            |-- Content
                |-- css
                    |-- shared.css
        |-- CoreModule
            |-- config.rb
            |-- Build
                |-- gruntfile.js
                |-- package.json
            |-- Sass
                |-- core-module.scss
            |-- Content
                |-- css
                    |-- core-module.css
```

### Shared Module
The Shared module is a slight exception to the rules of most of the other modules, because it contains elements and styles that are common to the rest of the modules. The Shared.css will always be required when accessing a module, so mostly the commonalites are handled by the cascade, but in some cases (such as extending existing styles for a special case) you can access needed sections within a module by using the @import directive and transversing the file structure.

For example:
``` scss
// Importing Compass
@import "compass";

// Accessing Shared styles 
@import "../../Shared/Sass/third-party/*";
@import "../../Shared/Sass/libs/*";
@import "../../Shared/Sass/config/*";

// Local components
@import "../Js/UIComponents/**/*";

// Other local styles
@import "outer-site-viewer";
```


### Configuring Build Process
#### Local Build
#### Global Build
Global build process requires the modules to be added to the config. 

## Global and Base Styles

### Extending common-base 
Many elements and components will share some common base styles. Common-base styles should be extremely granular and context-ignorant. These are styles that will be @extended and used elsewhere, though they may be allowed to apply directly as well. 

### What we mean by Global
There are some elements of Cofoundry which need styling, but which are not components. These are Global elements, and include things like the document root, and the general page layout.

## Component Styles

### Distributed file structure
Cofoundry has been built to be extremely modular, splitting markup and scripts into components. Rather than duplicating the folder structure of these components, we are experimenting with putting component styles in the same file structure as the rest of the component files, so the Angular template, script and styles will live in the same place.

This results in the Sass being spread around the project, but it will all be compiled down into one file, as normal. In theory it should make maintenance easier, as if you are working on a component, you know it's styles will be living in the same space as the rest of the component files.

### Scoping
It is important to keep component styles appropriately scoped so they don't accidentally affect other sections of the interface. 

To that end, we will be using a varient of BEM convention, where the Block is essentially the Component. 
