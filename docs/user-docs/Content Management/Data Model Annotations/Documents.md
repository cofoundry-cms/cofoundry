There are two different types of document data model annotations:

- [`[Document]`](#document)
- [`[DocumentCollection]`](#documentcollection)

Each of these are explained below:

## [Document]

The `[Document]` data annotation can be used to decorate an integer to indicate this is for the id of a document asset. 

A nullable integer indicates this is an optional field, while a non-null integer indicates this is a required field. 

#### Optional parameters

- **Tags:** Filters the document selection to only show documents with tags that match this value.
- **FileExtensions:** Filters the document selection to only show documents with these file extensions.

#### Example

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    /// <summary>
    /// A non nullable property indicated the document is required.
    /// </summary>
    [Document]
    public int ExampleRequiredDocument { get; set; }

    /// <summary>
    /// A nullable property indicates the document is optional.
    /// </summary>
    [Document]
    public int? ExampleOptionalDocument { get; set; }

    /// <summary>
    /// This document selection is filtered to documents with these specific
    /// file extensions.
    /// </summary>
    [Document(FileExtensions = new string[] { "pdf", "doc", "docx" })]
    public int? ExamplePdfOrWordDocument { get; set; }

    /// <summary>
    /// This document selection is filtered to documents labelled with these
    /// specific tags.
    /// </summary>
    [Document("Data Sheets", "Specifications")]
    public int? ExampleTagDocument { get; set; }
}
```

## [DocumentCollection]

The `[DocumentCollection]` data annotation can be used to decorate a collection of integers, indicating the property represents a set of document asset ids. The editor allows for sorting and you can set filters for restricting file types.

#### Optional parameters

- **Tags:** Filters the document selection to only show documents with tags that match this value.
- **FileExtensions:** Filters the document selection to only show documents with these file extensions.

#### Example

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    /// <summary>
    /// Default use, with no filters.
    /// </summary>
    [DocumentCollection]
    public ICollection<int> DocumentIds { get; set; }

    /// <summary>
    /// This document selection is filtered to documents with these specific
    /// file extensions.
    /// </summary>
    [DocumentCollection(FileExtensions = new string[] { "pdf", "doc", "docx" })]
    public int? PdfOrWordDocumentIds { get; set; }

    /// <summary>
    /// This document selection is filtered to documents labelled with these
    /// specific tags.
    /// </summary>
    [DocumentCollection(Tags = new string[] { "Data Sheets", "Specifications" })]
    public int? FilteredByTagDocumentIds { get; set; }
}
```