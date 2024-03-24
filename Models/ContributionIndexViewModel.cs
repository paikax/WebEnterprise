using System.Collections.Generic;
using WebEnterprise.Models;

namespace WebEnterprise.ViewModels
{
    public class ContributionIndexViewModel
    {
        public IEnumerable<Contribution> Contributions { get; set; }
        public IEnumerable<ContributionClosureDate> ClosureDates { get; set; }
        public IEnumerable<SchoolSystemData> SchoolSystemDatas  { get; set; }
    }
}
