using System.Threading.Tasks;
using PublicApiGenerator;
using VerifyXunit;
using Xunit;

namespace ImageListView.Tests
{
    public class PublicApiTests
    {
        [Fact]
        public Task PublicApiShouldNotChange()
        {
            var publicApi = typeof(Manina.Windows.Forms.ImageListView).Assembly
                .GeneratePublicApi();

            return Verifier.Verify(publicApi)
                .UniqueForTargetFrameworkAndVersion();
        }
    }
}