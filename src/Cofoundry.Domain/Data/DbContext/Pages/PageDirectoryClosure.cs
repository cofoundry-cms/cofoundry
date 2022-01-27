namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// <para>
    /// A "closure table" for the page directory heirarchy structure that connects
    /// every page directory with each of it's ancestor directories. The table also
    /// includes a self-referencing node (the same ancestor and descendant id).
    /// </para>
    /// <para>
    /// This table is automatically updated whenever changes are made to the page 
    /// directory heirarchy and should be treated as read-only.
    /// </para>
    /// </summary>
    public class PageDirectoryClosure
    {
        /// <summary>
        /// The id of a page directory that is an ancestor of <see cref="DescendantPageDirectoryId"/>. 
        /// The table includes a self-referencing node, so this can also be the same value as the 
        /// <see cref="DescendantPageDirectoryId"/>.
        /// </summary>
        public int AncestorPageDirectoryId { get; set; }

        /// <summary>
        /// The page directory that is an ancestor of <see cref="DescendantPageDirectory"/>. 
        /// The table includes a self-referencing node, so this can also be the same as 
        /// <see cref="DescendantPageDirectory"/>.
        /// </summary>
        public virtual PageDirectory AncestorPageDirectory { get; set; }

        /// <summary>
        /// The id of page directory that is a descendant of <see cref="AncestorPageDirectoryId"/>. 
        /// The table includes a self-referencing node, so this can also be the same value as the 
        /// <see cref="AncestorPageDirectoryId"/>.
        /// </summary>
        public int DescendantPageDirectoryId { get; set; }

        /// <summary>
        /// The page directory that is a descendant of <see cref="AncestorPageDirectory"/>. 
        /// The table includes a self-referencing node, so this can also be the same as 
        /// <see cref="AncestorPageDirectory"/>.
        /// </summary>
        public virtual PageDirectory DescendantPageDirectory { get; set; }

        /// <summary>
        /// The number of levels between the <see cref="AncestorPageDirectory"/> and the
        /// <see cref="DescendantPageDirectory"/>. For paths connected to the root node, this 
        /// will be the depth of the directory in the tree structure, and for the self-referencing 
        /// node this will be zero.
        /// </summary>
        public int Distance { get; set; }
    }
}