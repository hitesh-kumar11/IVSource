using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaThreeAdvisories: MemberPageBase
    {
       public List<IvsVisaSaarcDetails> IvsVisaSaarcDetailsList { get; set; }
       public List<IvsVisaCountryAdvisories> IvsVisaCountryAdvisorieslist { get; set; }
    }
}
