using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class EmbeddedResourceFileSourceTests
    {
        [Theory]
        [InlineData("Parent.Sub-Dir", "My-Lib.min.js", "Parent.Sub_Dir.My-Lib.min.js")]
        [InlineData(".Parent.Sub-Dir.", ".My-Lib.min.js", "Parent.Sub_Dir..My-Lib.min.js")]
        public void Ctor_FormatsFullPath(string path, string fileName, string expected)
        {
            var assembly = this.GetType().Assembly;
            var instance = new EmbeddedResourceFileSource(assembly, path, fileName);

            Assert.Equal(expected, instance.FullPath);
            Assert.Equal(fileName, instance.FileName);
            Assert.Equal(assembly, instance.Assembly);
        }

        [Fact]
        public async Task OpenReadStreamAsync_WhenExists_OpensStream()
        {
            var assembly = this.GetType().Assembly;
            var instance = new EmbeddedResourceFileSource(assembly, "Cofoundry.Domain.Tests.Domain.Shared.Models.Assets..Test-_Resouce!Directory()[]", ".Test re.source-!.txt");
            using var stream = await instance.OpenReadStreamAsync();

            Assert.NotNull(stream);
            Assert.True(stream.Length > 0);
        }

        [Fact]
        public async Task OpenReadStreamAsync_WhenNotExists_OpensStream()
        {
            var assembly = this.GetType().Assembly;
            var instance = new EmbeddedResourceFileSource(assembly, "Domain.Shared.Models.Assets", "NotFound.txt");
            using var stream = await instance.OpenReadStreamAsync();

            Assert.Null(stream);
        }
    }
}
