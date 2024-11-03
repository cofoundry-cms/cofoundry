using Cofoundry.Domain;
using Xunit;

namespace Cofoundry.Plugins.Imaging.SkiaSharp.Tests;

public class ResizeSpecificationFactoryTests
{
    public static readonly object[][] AllImageParameters = [[TestImages.Landscape_2048x1375], [TestImages.Portrait_1536x2048]];

    [Theory]
    [MemberData(nameof(AllImageParameters))]
    public void Create_WhenNotResized_DoesNotAlter(TestImage testImage)
    {
        var resizeSettings = new ImageResizeSettings();
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(testImage.Width, result.CanvasWidth);
        Assert.Equal(testImage.Width, result.UncroppedImageWidth);
        Assert.Equal(testImage.Width, result.VisibleImageWidth);
        Assert.Equal(testImage.Height, result.CanvasHeight);
        Assert.Equal(testImage.Height, result.UncroppedImageHeight);
        Assert.Equal(testImage.Height, result.VisibleImageHeight);
    }

    #region crop

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenCropWidthOnly_CropsCorrectly(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Crop,
            Scale = imageScaleMode,
            Width = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(500, result.CanvasWidth);
        Assert.Equal(500, result.UncroppedImageWidth);
        Assert.Equal(500, result.VisibleImageWidth);
        Assert.Equal(336, result.CanvasHeight);
        Assert.Equal(336, result.UncroppedImageHeight);
        Assert.Equal(336, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenCropHeightOnly_CropsCorrectly(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Crop,
            Scale = imageScaleMode,
            Height = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(745, result.CanvasWidth);
        Assert.Equal(745, result.UncroppedImageWidth);
        Assert.Equal(745, result.VisibleImageWidth);
        Assert.Equal(500, result.CanvasHeight);
        Assert.Equal(500, result.UncroppedImageHeight);
        Assert.Equal(500, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenCropWithShortWidth_WidthCutoff(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Crop,
            Scale = imageScaleMode,
            Width = 500,
            Height = 1000
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(500, result.CanvasWidth);
        Assert.Equal(1489, result.UncroppedImageWidth);
        Assert.Equal(500, result.VisibleImageWidth);
        Assert.Equal(1000, result.CanvasHeight);
        Assert.Equal(1000, result.UncroppedImageHeight);
        Assert.Equal(1000, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenCropWithShortHeight_HeightCutoff(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Crop,
            Scale = imageScaleMode,
            Width = 1000,
            Height = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(1000, result.CanvasWidth);
        Assert.Equal(1000, result.UncroppedImageWidth);
        Assert.Equal(1000, result.VisibleImageWidth);
        Assert.Equal(500, result.CanvasHeight);
        Assert.Equal(671, result.UncroppedImageHeight);
        Assert.Equal(500, result.VisibleImageHeight);
    }

    #endregion

    #region pad

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenPadWidthOnly_DoesNotPad(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Pad,
            Scale = imageScaleMode,
            Width = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(500, result.CanvasWidth);
        Assert.Equal(500, result.UncroppedImageWidth);
        Assert.Equal(500, result.VisibleImageWidth);
        Assert.Equal(336, result.CanvasHeight);
        Assert.Equal(336, result.UncroppedImageHeight);
        Assert.Equal(336, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenPadHeightOnly_DoesNotPad(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Pad,
            Scale = imageScaleMode,
            Height = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(745, result.CanvasWidth);
        Assert.Equal(745, result.UncroppedImageWidth);
        Assert.Equal(745, result.VisibleImageWidth);
        Assert.Equal(500, result.CanvasHeight);
        Assert.Equal(500, result.UncroppedImageHeight);
        Assert.Equal(500, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenPadWithShortWidth_HeightPadded(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Pad,
            Scale = imageScaleMode,
            Width = 500,
            Height = 1000
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(500, result.CanvasWidth);
        Assert.Equal(500, result.UncroppedImageWidth);
        Assert.Equal(500, result.VisibleImageWidth);
        Assert.Equal(1000, result.CanvasHeight);
        Assert.Equal(336, result.UncroppedImageHeight);
        Assert.Equal(336, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenPadWithShortHeight_WidthPadded(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Pad,
            Scale = imageScaleMode,
            Width = 1000,
            Height = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(1000, result.CanvasWidth);
        Assert.Equal(745, result.UncroppedImageWidth);
        Assert.Equal(745, result.VisibleImageWidth);
        Assert.Equal(500, result.CanvasHeight);
        Assert.Equal(500, result.UncroppedImageHeight);
        Assert.Equal(500, result.VisibleImageHeight);
    }

    #endregion

    #region max

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenMaxWithWidthOnly_Resizes(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Max,
            Scale = imageScaleMode,
            Width = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(500, result.CanvasWidth);
        Assert.Equal(500, result.UncroppedImageWidth);
        Assert.Equal(500, result.VisibleImageWidth);
        Assert.Equal(336, result.CanvasHeight);
        Assert.Equal(336, result.UncroppedImageHeight);
        Assert.Equal(336, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenMaxWithHeightOnly_Resizes(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Max,
            Scale = imageScaleMode,
            Height = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(745, result.CanvasWidth);
        Assert.Equal(745, result.UncroppedImageWidth);
        Assert.Equal(745, result.VisibleImageWidth);
        Assert.Equal(500, result.CanvasHeight);
        Assert.Equal(500, result.UncroppedImageHeight);
        Assert.Equal(500, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenMaxWithShortWidth_Resizes(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Max,
            Scale = imageScaleMode,
            Width = 500,
            Height = 1000
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(500, result.CanvasWidth);
        Assert.Equal(500, result.UncroppedImageWidth);
        Assert.Equal(500, result.VisibleImageWidth);
        Assert.Equal(336, result.CanvasHeight);
        Assert.Equal(336, result.UncroppedImageHeight);
        Assert.Equal(336, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageScaleMode.DownscaleOnly)]
    [InlineData(ImageScaleMode.UpscaleCanvas)]
    [InlineData(ImageScaleMode.Both)]
    public void Create_WhenMaxWithShortHeight_Resizes(ImageScaleMode imageScaleMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = ImageFitMode.Max,
            Scale = imageScaleMode,
            Width = 1000,
            Height = 500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(745, result.CanvasWidth);
        Assert.Equal(745, result.UncroppedImageWidth);
        Assert.Equal(745, result.VisibleImageWidth);
        Assert.Equal(500, result.CanvasHeight);
        Assert.Equal(500, result.UncroppedImageHeight);
        Assert.Equal(500, result.VisibleImageHeight);
    }

    #endregion

    #region ImageScaleMode.DownscaleOnly

    [Theory]
    [InlineData(ImageFitMode.Crop)]
    [InlineData(ImageFitMode.Pad)]
    [InlineData(ImageFitMode.Max)]
    public void Create_WhenDownscaleOnlyAndWidthOnly_DoesNotUpscale(ImageFitMode fitMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = fitMode,
            Scale = ImageScaleMode.DownscaleOnly,
            Width = 2500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(2048, result.CanvasWidth);
        Assert.Equal(2048, result.UncroppedImageWidth);
        Assert.Equal(2048, result.VisibleImageWidth);
        Assert.Equal(1375, result.CanvasHeight);
        Assert.Equal(1375, result.UncroppedImageHeight);
        Assert.Equal(1375, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageFitMode.Crop)]
    [InlineData(ImageFitMode.Pad)]
    [InlineData(ImageFitMode.Max)]
    public void Create_WhenDownscaleOnlyAndHeightOnly_DoesNotUpscale(ImageFitMode fitMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = fitMode,
            Scale = ImageScaleMode.DownscaleOnly,
            Height = 2500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(2048, result.CanvasWidth);
        Assert.Equal(2048, result.UncroppedImageWidth);
        Assert.Equal(2048, result.VisibleImageWidth);
        Assert.Equal(1375, result.CanvasHeight);
        Assert.Equal(1375, result.UncroppedImageHeight);
        Assert.Equal(1375, result.VisibleImageHeight);
    }

    #endregion

    #region ImageScaleMode.UpscaleCanvas

    [Theory]
    [InlineData(ImageFitMode.Crop)]
    [InlineData(ImageFitMode.Pad)]
    [InlineData(ImageFitMode.Max)]
    public void Create_WhenUpscaleCanvasAndCropAndWidthOnly_UpscalesCanvasOnly(ImageFitMode fitMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = fitMode,
            Scale = ImageScaleMode.UpscaleCanvas,
            Width = 2500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(2500, result.CanvasWidth);
        Assert.Equal(2048, result.UncroppedImageWidth);
        Assert.Equal(2048, result.VisibleImageWidth);
        Assert.Equal(1375, result.CanvasHeight);
        Assert.Equal(1375, result.UncroppedImageHeight);
        Assert.Equal(1375, result.VisibleImageHeight);
    }

    [Theory]
    [InlineData(ImageFitMode.Crop)]
    [InlineData(ImageFitMode.Pad)]
    [InlineData(ImageFitMode.Max)]
    public void Create_WhenUpscaleCanvasAndCropAndHeightOnly_UpscalesCanvasOnly(ImageFitMode fitMode)
    {
        var resizeSettings = new ImageResizeSettings()
        {
            Mode = fitMode,
            Scale = ImageScaleMode.UpscaleCanvas,
            Height = 2500
        };

        var testImage = TestImages.Landscape_2048x1375;
        var result = CreateSpecification(testImage, resizeSettings);

        Assert.Equal(2048, result.CanvasWidth);
        Assert.Equal(2048, result.UncroppedImageWidth);
        Assert.Equal(2048, result.VisibleImageWidth);
        Assert.Equal(2500, result.CanvasHeight);
        Assert.Equal(1375, result.UncroppedImageHeight);
        Assert.Equal(1375, result.VisibleImageHeight);
    }

    #endregion

    private static ResizeSpecification CreateSpecification(TestImage image, ImageResizeSettings imageResizeSettings)
    {
        var factory = new ResizeSpecificationFactory();

        using var imageFile = image.Load();

        if (!imageFile.IsLoaded)
        {
            throw new InvalidOperationException($"{nameof(imageFile)} is expected to be loaded");
        }

        return factory.Create(imageFile.Codec, imageFile.Bitmap, imageResizeSettings);
    }
}
