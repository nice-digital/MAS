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
        public static string ReceiveEverythingGroupId = "send-me-everything";

        private static string[] allSpecialities = new string[] {
                        "Allergy and immunology", "Anaesthesia and pain", "Cancers", "Cardiovascular system disorders", "Complementary and alternative therapies", "Critical care", "Diabetes", "Ear, nose and throat disorders", "Emergency medicine and urgent care", "Endocrine system disorders", "Eyes and vision", "Family planning", "Gastrointestinal disorders", "Genetics", "Haematological disorders", "Infection and infectious diseases", "Later life", "Learning disabilities", "Liver disorders", "Mental health and illness", "Musculo-skeletal disorders", "Neurological disorders", "Nutritional and metabolic disorders", "Obstetrics and gynaecology", "Oral and dental health", "Other / Unclassified", "Paediatric and neonatal medicine", "Palliative and End of Life Care", "Policy, Commissioning and Managerial", "Renal and urologic disorders", "Respiratory disorders", "Sexual health", "Skin disorders", "Sports medicine", "Stroke", "Surgery", "Travel medicine", "Vaccination", "Wounds and injuries"
                    };

        public static IEnumerable<Interest> GetAllSpecialityInterests()
        {
            return allSpecialities.Select(s => new Interest { Name = s });
        }

        public FakeMailChimpManager()
        {
            Setup(s => s.InterestCategories.GetAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.SpecialityCategoryId))
                .ReturnsAsync(new InterestCategory { Title = "Daily specialities of interest", Id = TestAppSettings.MailChimp.Default.SpecialityCategoryId });

            Setup(s => s.InterestCategories.GetAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId))
                .ReturnsAsync(new InterestCategory { Title = "Send me everything from Medicines Awareness Daily", Id = TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId });

            Setup(s => s.Interests.GetAllAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.ReceiveEverythingCategoryId, null))
                .ReturnsAsync(new List<Interest> {
                    new Interest { Name = "Send me everything", Id = ReceiveEverythingGroupId }
                });

            Setup(s => s.Interests.GetAllAsync(TestAppSettings.MailChimp.Default.ListId, TestAppSettings.MailChimp.Default.SpecialityCategoryId, null))
                .ReturnsAsync(() => GetAllSpecialityInterests());

            Setup(x => x.Campaigns.AddAsync(It.IsAny<Campaign>()))
                .ReturnsAsync(new Campaign() { Id = "1234" });

            Setup(x => x.Content.AddOrUpdateAsync("1234", It.IsAny<ContentRequest>()));
        }
    }
}
