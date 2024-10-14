using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaNoteFees : MemberPageBase
    {
        public int SerialNum { get; set; }
        public int Priority { get; set; }
        public string CountryIso { get; set; }
        public string CityId { get; set; }
        public string VisaInformation { get; set; }       
        public string VisaInformationDocumentsRequired { get; set; }
        public string VisaInformationProcessingTime { get; set; }
        public string VisaGeneralInformation { get; set; }
        public string VisaCategoryId { get; set; }
        public string VisaCategory { get; set; }
        public string VisaCategoryCode { get; set; }
        public string VisaCategoryInformation { get; set; }
        public string VisaCategoryInformationVisaProcedure { get; set; }
        public string VisaCategoryInformationDocumentsRequired { get; set; }
        public string VisaCategoryInformationProcessingTime { get; set; }
        public string VisaCategoryNotes { get; set; }
        public string VisaCategoryRequirements { get; set; }
        public string VisaCategoryOption { get; set; }
        public string VisaCategoryOptionCode { get; set; }
        public string VisaCategoryOptionAmountInr { get; set; }
        public string VisaCategoryOptionAmountOther { get; set; }
        public DateTime? VisaCategoryModifiedDate { get; set; }
        public string VisaCategoryForm { get; set; }
        public string VisaCategoryFormPath { get; set; }
        public string Email { get; set; }

        public int? IsEnable { get; set; }
    }

    public class IvsVisaNoteFeesList : MemberPageBase
    {
        public List<IvsVisaNoteFees> IvsVisaNotes { get; set; }
        public List<IvsVisaNoteFees> countryDetailsInformation { get; set; }
        public List<IvsVisaNoteFees> DistinctIvsVisaNotesCat { get; set; }
        public List<IvsVisaNoteFees> OrderByIvsVisaNotesCat { get; set; }
        public List<IvsVisaNoteFees> DistinctIvsVisaNotesForm { get; set; }
        public List<IvsVisaNoteFees> DistinctIvsVisaNotes { get; set; }
        public List<IvsVisaCountryTerritoryCities> coTerritoryCities { get; set; }
        public string CityName { get; set; }
        public string CityId { get; set; }
        // public List<IvsVisaCountryTerritoryCities> TerritoryCities { get; set; }
    }

    public class IvsVisaNoteAndFeesList : MemberPageBase
    {
        public List<IvsVisaNoteFees> CityDetails { get; set; }
        public List<IvsVisaNoteFees> CityDetailsInformation { get; set; }
        public List<IvsVisaNoteFees> CityCategoryDetails { get; set; }
        public List<IvsVisaNoteFees> CityCategoryFees { get; set; }
        public List<IvsVisaNoteFees> CityFormDetails { get; set; }
        public string CityName { get; set; }
        public string CityId { get; set; }
    }
}
