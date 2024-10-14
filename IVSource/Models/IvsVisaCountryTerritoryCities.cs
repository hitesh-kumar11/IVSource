using System;
using System.Collections.Generic;

namespace IVSource.Models
{
    public partial class IvsVisaCountryTerritoryCities : MemberPageBase
    {
        public int SerialNum { get; set; }
        public string CityId { get; set; }
        public string CountryIso { get; set; }
        public string CityName { get; set; }
        public string CityIso { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? IsEnable { get; set; }
    }
}
