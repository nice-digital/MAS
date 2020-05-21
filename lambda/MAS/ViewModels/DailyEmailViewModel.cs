using MAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAS.ViewModels
{
    public class DailyEmailViewModel
    {
        public IList<Item> Items { get; set; }

        public string SpecialitiesGroupCategoryName { get; set; }

        /// <summary>
        /// The name of the MailChimp group category for receiving everything in the daily
        /// email, for example 'Send me everything from Medicines Awareness Daily'. But we
        /// look it up by id from MailChimp, rather than hardcoding it, so that it can change.
        /// </summary>
        public string EverythingGroupCategoryName { get; set; }

        /// <summary>
        /// The name of the single group for receiving everything in the daily.
        /// </summary>
        public string EverythingGroupName { get; set; }

        /// <summary>
        /// List of all specialities from MailChimp.
        /// </summary>
        public string AllSpecialities { get; set; }
    }
}
