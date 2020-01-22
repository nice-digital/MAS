using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MAS.Tests.Infrastructure;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace MAS.Tests.Fakes
{
    public class FakeMailChimpManager : Mock<IMailChimpManager>
    {
        #region Statics for use in assertions

        // Campaign
        public static string CampaignId = "1234";
        public static Campaign Campaign = new Campaign { Id = CampaignId };

        // Receive everything group category
        public static string ReceiveEverythingGroupCategoryName = "Send me everything from Medicines Awareness Daily";
        public static InterestCategory ReceiveEverythingGroupCategory = new InterestCategory
        {
            Title = ReceiveEverythingGroupCategoryName,
            Id = TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId
        };

        // Receive everything group
        public static string ReceiveEverythingGroupName = "Send me everything";
        public static string ReceiveEverythingGroupId = "send-me-everything";
        public static Interest ReceiveEverythingGroup = new Interest
            {
                Name = ReceiveEverythingGroupName,
                Id = ReceiveEverythingGroupId
            };

        // Specialities group category
        public static string SpecialitiesGroupCategoryName = "Daily specialities of interest";
        public static InterestCategory SpecialitiesGroupCategory = new InterestCategory
        {
            Title = SpecialitiesGroupCategoryName,
            Id = TestAppSettings.MailChimp.Default.SpecialityCategoryId
        };

        // Specialities
        private static string[] allSpecialities = new string[] {
                        "Allergy and immunology", "Anaesthesia and pain", "Cancers", "Cardiovascular system disorders", "Complementary and alternative therapies", "Critical care", "Diabetes", "Ear, nose and throat disorders", "Emergency medicine and urgent care", "Endocrine system disorders", "Eyes and vision", "Family planning", "Gastrointestinal disorders", "Genetics", "Haematological disorders", "Infection and infectious diseases", "Later life", "Learning disabilities", "Liver disorders", "Mental health and illness", "Musculo-skeletal disorders", "Neurological disorders", "Nutritional and metabolic disorders", "Obstetrics and gynaecology", "Oral and dental health", "Other / Unclassified", "Paediatric and neonatal medicine", "Palliative and End of Life Care", "Policy, Commissioning and Managerial", "Renal and urologic disorders", "Respiratory disorders", "Sexual health", "Skin disorders", "Sports medicine", "Stroke", "Surgery", "Travel medicine", "Vaccination", "Wounds and injuries"
                    };
        public static IEnumerable<Interest> GetAllSpecialityInterests()
        {
            return allSpecialities.Select((s, i) => new Interest {
                Name = s,
                // Just use a numeric id we can use easily within tests
                Id = (i + 1).ToString()
            });
        }

        #endregion

        public FakeMailChimpManager()
        {
            Setup(s => s.InterestCategories.GetAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.SpecialityCategoryId))
                .ReturnsAsync(SpecialitiesGroupCategory);

            Setup(s => s.InterestCategories.GetAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId))
                .ReturnsAsync(ReceiveEverythingGroupCategory);

            Setup(s => s.Interests.GetAllAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId, null))
                .ReturnsAsync(new List<Interest> { ReceiveEverythingGroup });

            Setup(s => s.Interests.GetAllAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.SpecialityCategoryId, null))
                .ReturnsAsync(() => GetAllSpecialityInterests());

            Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(Campaign);

            Setup(x => x.Content.AddOrUpdateAsync(CampaignId, It.IsAny<ContentRequest>()));
        }
    }
}
