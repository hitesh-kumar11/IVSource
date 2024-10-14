using System;
using System.Collections.Generic;

namespace IVSource.Models
{
    public partial class IvsVisaCountriesAlias
    {
        public int SerialNum { get; set; }
        public string CountryName { get; set; }
        public string CountryIso { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? IsEnable { get; set; }
    }
}
