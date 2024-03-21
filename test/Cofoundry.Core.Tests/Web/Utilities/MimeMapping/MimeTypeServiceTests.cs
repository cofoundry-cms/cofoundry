﻿using Cofoundry.Core.Web.Internal;
using Microsoft.AspNetCore.StaticFiles;
using NSubstitute;

namespace Cofoundry.Core.Tests;

public class MimeTypeServiceTests
{
    const string JPEG_MIME_TYPE = "image/jpeg";
    const string OCTET_STREAM_MIME_TYPE = "application/octet-stream";
    const string TEST_DEFAULT_MIME_TYPE = "application/wibble";

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("     ")]
    public void MapFromFileName_WhenEmpty_ThrowsArgumentException(string? fileName)
    {
        var contentTypeProvider = Substitute.For<IContentTypeProvider>();
        var service = new MimeTypeService(contentTypeProvider);

        Assert.ThrowsAny<ArgumentException>(() => service.MapFromFileName(fileName!));
    }

    [Fact]
    public void MapFromFileName_WhenContentTypeFound_ReturnsMatch()
    {
        var fileName = "my-file.jpg";
        var contentType = JPEG_MIME_TYPE;

        var contentTypeProvider = Substitute.For<IContentTypeProvider>();
        contentTypeProvider
            .TryGetContentType(Arg.Is(fileName), out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = contentType;
                return true;
            });
        var service = new MimeTypeService(contentTypeProvider);

        var result = service.MapFromFileName(fileName);

        Assert.Equal(contentType, result);
    }

    [Fact]
    public void MapFromFileName_WhenContentTypeNotFound_ReturnsDefault()
    {
        var fileName = "my-file.jpg";
        string? contentType = null;

        var contentTypeProvider = Substitute.For<IContentTypeProvider>();
        contentTypeProvider
            .TryGetContentType(Arg.Is(fileName), out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = contentType;
                return false;
            });
        var service = new MimeTypeService(contentTypeProvider);

        var result = service.MapFromFileName(fileName);

        Assert.Equal(OCTET_STREAM_MIME_TYPE, result);
    }


    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("     ")]
    public void MapFromFileNameWithDefault_WhenEmpty_ThrowsArgumentException(string? fileName)
    {
        var contentTypeProvider = Substitute.For<IContentTypeProvider>();
        var service = new MimeTypeService(contentTypeProvider);

        Assert.ThrowsAny<ArgumentException>(() => service.MapFromFileName(fileName!, TEST_DEFAULT_MIME_TYPE));
    }

    [Fact]
    public void MapFromFileNameWithDefault_WhenContentTypeFound_ReturnsMatch()
    {
        var fileName = "my-file.jpg";
        var contentType = JPEG_MIME_TYPE;

        var contentTypeProvider = Substitute.For<IContentTypeProvider>();
        contentTypeProvider
            .TryGetContentType(Arg.Is(fileName), out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = contentType;
                return true;
            });
        var service = new MimeTypeService(contentTypeProvider);

        var result = service.MapFromFileName(fileName, TEST_DEFAULT_MIME_TYPE);

        Assert.Equal(contentType, result);
    }

    [Fact]
    public void MapFromFileNameWithDefault_WhenContentTypeNotFound_ReturnsDefault()
    {
        var fileName = "my-file.jpg";
        string? contentType = null;

        var contentTypeProvider = Substitute.For<IContentTypeProvider>();
        contentTypeProvider
            .TryGetContentType(Arg.Is(fileName), out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = contentType;
                return false;
            });
        var service = new MimeTypeService(contentTypeProvider);

        var result = service.MapFromFileName(fileName, TEST_DEFAULT_MIME_TYPE);

        Assert.Equal(TEST_DEFAULT_MIME_TYPE, result);
    }
}
