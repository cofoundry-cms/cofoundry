# PageBlockTypes Sample

A bare website showing various examples of how to implement page block types.

#### To get started:

1. Start up the local-env docker compose file to get the database running. See [local-env docs](../../local-env/README.md) for details.
2. Run the website and navigate to *"/admin"*, which will display the setup screen
3. Enter an application name and setup your user account. Submit the form to complete the site setup. 
4. Sign in
5. Navigate to the homepage where you will be able to use the visual editor to configure page block types.

####  Example Page Block Types:

- **Carousel:** A multi-item carousel making use of `NestedDataModelCollection` to allow editing of a collection of slides.
- **ContentSection:** An example of a block type hosted in a separate project
- **DirectoryList:** Lists pages in a specific directory. Demonstrates searching for pages using `IPageRepository` and using the `WebDirectoryAttribute` data model attribute.
- **HorizontalLine:** A very simple block type that adds a variable width horizontal line (wrapped hr tag).
- **PageList:** An orderable list of links to pages. Demonstrates querying for cached page routes using `IPageRepository`, the `PageCollectionAttribute` data model attribute and generating links to pages from page objects.
- **PageSnippet:** Displays summary information about a page. Demonstrates the `PageAttribute` data model attribute, querying and manipulating block data and handling availability of linked entities (due to draft status).
- **Quotation:** A quotation block that is output as blockquote with an optional cite tag.
- **SocialProfiles:** An example of using `NestedDataModelMultiTypeCollection` to create a list of social media profile links by utilizing four different types of nested data models in one collection.
- **TextList:** A list of text items that displays in an html unordered list, or optional in an ordered list.

## ExampleSharedProject

The solution also contains a project that demonstrates how to organize page blocks into a shared project. If you're implementing your own shared project for block types, there are some rules you need to follow:

- Ensure your project is named in a way it can be picked up by DI: check the relevant section of the [dependency injection docs](https://www.cofoundry.org/docs/framework/dependency-injection#registering-dependencies-in-other-assemblies) for more information.
- Ensure any view files are marked as embedded resources. This can be done either through the Visual Studio UI, or by adding a generic rule to your `.csproj` file:

```
<ItemGroup>
    <EmbeddedResource Include="**\*.cshtml;" />
</ItemGroup>
```
- Include an `IAssemblyResourceRegistration` implementation: This tells Cofoundry that your assembly contains embedded resources that need to included in the solution.
- Ensure view files are located in one of the default view block type folders, or implement your own `IPageBlockTypeViewLocationRegistration` if you want to namespace it under a custom path to avoid conflicts. See the [page block types docs](https://www.cofoundry.org/docs/content-management/page-block-types#file-location) for more info.
- Note that with embedded resources, files in your main project will override embedded resources in the same location in a dependent projects. This includes files like `_ViewImports.cshtml`, so it's best either to reference imports directly in your view file (as we have done in this example) or customize your block type view path to ensure it does not conflict.

