using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation
{
    // TODO: Is this required anymore?
    //public class FileExtensionsAttribute : RegularExpressionAttribute
    //{
    //    private const string regex = @"^.*({0})$";
    //    private const string fileRegex = @"\.{0}";
    //    private const string letterRegex = @"[{0}{1}]";
        
    //    public FileExtensionsAttribute(params string[] fileExtensions)
    //        : base(buildRegex(fileExtensions))
    //    {
    //        switch (fileExtensions.Length)
    //        {
    //            case 0:
    //                throw new ArgumentException("You must use at least one file extension");
    //            case 1:
    //                ErrorMessage = string.Format("Please upload a file with the extension .{0}", stripDot(fileExtensions[0]));
    //                break;
    //            default:
    //                string files = "";
    //                foreach (string extension in fileExtensions.Take(fileExtensions.Length - 1))
    //                {
    //                    files += "." + stripDot(extension);
    //                    if (fileExtensions.Take(fileExtensions.Length - 1).Last() != extension)
    //                        files += ", ";
    //                }
    //                ErrorMessage = string.Format("Please upload a file with either the extension {0} or .{1}", files, stripDot(fileExtensions.Last()));
    //                break;
    //        }
    //    }

    //    private static string stripDot(string extension)
    //    {
    //        return extension.Contains(".") ? extension.Substring(extension.LastIndexOf(".") + 1) : extension;
    //    }

    //    /**
    //     * E.g. .*(\.[Jj][Pp][Gg]|\.[Gg][Ii][Ff]|\.[Jj][Pp][Ee][Gg]|\.[Pp][Nn][Gg])
    //     */
    //    private static string buildRegex(string[] fileExtensions)
    //    {
    //        string files = "";
    //        foreach (string fileExtension in fileExtensions)
    //        {
    //            string extension = stripDot(fileExtension);

    //            string fileLetters = "";
    //            foreach(char letter in extension)
    //            {
    //                fileLetters += string.Format(letterRegex, char.ToUpper(letter), char.ToLower(letter));
    //            }

    //            files += string.Format(fileRegex, fileLetters);
    //            if (fileExtensions.Last() != fileExtension)
    //                files += "|";
    //        }

    //        return string.Format(regex, files);
    //    }

    //    public override bool IsValid(object value)
    //    {
    //        return base.IsValid(value);
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        return base.IsValid(value, validationContext);
    //    }
    //}
}
