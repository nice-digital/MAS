using System.Threading.Tasks;
using MAS.Models;
using MAS.Services;

namespace MAS.Tests.Infrastructure
{
    internal class FakeContentService : IContentService
    {
        public Task<Item> GetItemAsync(string itemId)
        {
            return Task.Run(() =>
            {
                return new Item()
                {
                    Id = itemId,
                    Title = "My Test Drug"
                };
            });
        }
    }
}