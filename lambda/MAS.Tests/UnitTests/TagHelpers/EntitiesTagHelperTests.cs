using MAS.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace MAS.Tests.UnitTests.TagHelpers
{
    /// <see cref="https://dotnetthoughts.net/unit-testing-aspnet-core-taghelper/"/>
    public class EntitiesTagHelperTests
    {
        [Fact]
        public async Task ReplacesUnicodeEntitiesWithHTMLEntities()
        {
            //Arrange
            EntitiesTagHelper tagHelper = new EntitiesTagHelper();

            TagHelperOutput tagHelperOutput = new TagHelperOutput("markdown",
                    new TagHelperAttributeList(),
                (result, encoder) =>
                    {
                        DefaultTagHelperContent tagHelperContent = new DefaultTagHelperContent();
                        tagHelperContent.SetHtmlContent("<p>A string with &nbsp; &epsilon; entities</p>");
                        return Task.FromResult<TagHelperContent>(tagHelperContent);
                    });

            //Act
            await tagHelper.ProcessAsync(null, tagHelperOutput);

            //Assert
            tagHelperOutput.Content.GetContent().ShouldBe("<p>A string with &#160; &#949; entities</p>");
        }
    }
}
