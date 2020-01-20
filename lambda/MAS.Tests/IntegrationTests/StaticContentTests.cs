using Amazon.S3;
using Amazon.S3.Model;
using MAS.Models;
using MAS.Tests.Infrastructure;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MAS.Tests.IntegrationTests
{
    public class StaticContentTests : TestBase
    {
        public StaticContentTests(ITestOutputHelper output) : base(output)
        { }

        private Item item = new Item()
        {
            Id = "1234",
            Slug = "effect-of-vit-d",
            URL = "www.website.com",
            Title = "Effect of Vitamin D and Omega-3 Fatty Acid Supplementation on Kidney Function in Patients With Type 2 Diabetes: A Randomized Clinical Trial",
            ShortSummary = "RCT (n=1,312) found that among adults with type 2 diabetes, supplementation with vitamin D3 or omega-3 fatty acids, compared with placebo, resulted in no significant difference in change in eGFR at 5 years.",
            Source = new Source()
            {
                Id = "789",
                Title = "Journal of the American Medical Association"
            },
            EvidenceType = new EvidenceType
            {
                Key = "mas_evidence_types:Safety%20alerts",
                Title = "Safety alerts"
            },
            Comment = "A related editorial discusses this research and details previous epidemiological studies that suggest improved outcomes with vitamin D supplementation in various clinical scenarios. It states that contrasting the results of this study and its predecessor vitamin D trials with the impressive body of epidemiological research that implicated vitamin D deficiency in various adverse health outcomes offers a stark lesson on the chasm between association and causation. Editorial authors highlight that it now seems safe to conclude that many prior epidemiological associations between vitamin D deficiency and adverse health outcomes were driven by unmeasured residual confounding or reverse causality.",
            ResourceLinks = "<p><a title=\"Link 1\" href=\"items/5de65fe432281d43fbfcd15a\">Vitamin D and Health Outcomes - Then Came the Randomized Clinical Trials</a></p>\r\n<p><a title=\"sadada\" href=\"items/5de65fe432281d43fbfcd15a\">Link 2</a></p>"

        };

        [Fact]
        public async Task PutCMSItemSavesItemIntoS3()
        {
            // Arrange
            var fakeS3Service = new Mock<IAmazonS3>();

            PutObjectRequest putObjectRequest = null;
            fakeS3Service.Setup(s => s.PutObjectAsync(It.IsAny<PutObjectRequest>(), default(CancellationToken)))
                .Callback<PutObjectRequest, CancellationToken>((pOR, cT) => putObjectRequest = pOR)
                .ReturnsAsync(new PutObjectResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

            var client = _factory.WithImplementation(fakeS3Service.Object).CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync("/api/content/", content);

            // Assert
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            putObjectRequest.Key.ShouldBe("effect-of-vit-d.html");
            putObjectRequest.ContentBody.ShouldMatchApproved();
        }
            
    }
}