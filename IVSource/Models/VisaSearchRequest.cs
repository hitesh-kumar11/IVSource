using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IVSource.Models
{
    //start response
    public class VisaSearchRequest : MemberPageBase
    {

        [JsonProperty("IVA_VisaSearchResponse")]
        public IVA_VisaSearchResponse IVA_VisaSearchResponse;
    }
    public class IVA_VisaSearchResponse
    {
        [JsonProperty("VisaSearchResponse")]
        public VisaSearchResponse VisaSearchResponse;
    }
    public class VisaSearchResponse
    {
        [JsonProperty("VisaDetails")]
        public VisaDetails VisaDetails;
    }
    public class VisaDetails
    {
        [JsonIgnore]
        [System.Xml.Serialization.XmlAttribute()]
        public string CountryCode { get; set; }
        //[JsonProperty("CountryName")]
        //[Key]
        //public List<CountryName> cNames { get; set; }
        //[JsonProperty("CountryDetails")]
        //public List<IvsVisaCountriesDetails> CountryDetails { get; set; }

        public CountryDetail CountryDetails { get; set; }

        //[JsonProperty("Holiydays")]
        //public List<IvsVisaCountriesHolidays> Holidays { get; set; }
        //[JsonProperty("Airlines")]
        //public List<IvsVisaCountriesAirlines> Airlines { get; set; }
        //[JsonProperty("Airports")]
        //public List<IvsVisaCountriesAirports> Airports { get; set; }
        //[JsonProperty("DiplomaticRepresentation")]
        //public List<IvsVisaCountriesDiplomaticRepresentation> DiplomaticRepresentation { get; set; }
        public DiplomaticRepresentation DiplomaticRepresentation { get; set; }
        [JsonProperty("IndianEmbassy")]
        public List<IvsVisaHelpAddress> IndianEmbassy { get; set; }
        //[JsonProperty("IntlHelpAddress")]
        //public List<IvsVisaHelpAddress> IntlHelpAddress { get; set; }
        [XmlElement("IntlHelpAddress")]
        [JsonProperty("IntlHelpAddress")]
        public IntlHelpAddress IntlHelpAddress { get; set; }
        [JsonProperty("IVSAdvisory")]
        [XmlElement("IVSAdvisory")]
        public Descriptions IVSAdvisory { get; set; }

        [JsonProperty("ReciprocalVisaInfo")]
        //   public List<IvsVisaCountryAdvisories> ReciprocalVisaInfo { get; set; }
        public Descriptions ReciprocalVisaInfo { get; set; }
        [JsonProperty("InternationalAdvisory")]
        //  public List<IvsVisaCountryAdvisories> InternationalAdvisory { get; set; }
        public Descriptions InternationalAdvisory { get; set; }
        [JsonProperty("SAARCInfo")]
        [XmlElement("SAARCInfo")]
        //public List<IvsVisaSaarcDetails> SAARCInfo { get; set; }
        public SAARCInfo SAARCInfo { get; set; }
        [JsonProperty("VisaInformation")]
        public List<IvsVisaInformation> Visainf { get; set; }
        [JsonProperty("Categories")]
        public List<IvsVisaCategories> Categories { get; set; }
        [JsonProperty("CategoryFees")]
        public List<IvsVisaCategoriesOptions> CategoryFees { get; set; }
        [JsonProperty("CategoryForm")]
        public List<IvsVisaCategoriesForms> CategoriesForm { get; set; }
        [JsonProperty("AdditionalInfo")]
        [XmlElement("AdditionalInfo")]
        public string AdditionalInfo { get; set; }
    }
    //
    public class CountryDetail
    {
        [JsonProperty("CountryName")]
        [XmlElement("CountryName")]
        [Key]
        public List<CountryName> cNames { get; set; }
        [XmlElement("GeneralInfo")]
        [JsonProperty("GeneralInfo")]
        public List<IvsVisaCountriesDetails> GeneralInfo { get; set; }
        [JsonProperty("Holiydays")]
        public List<IvsVisaCountriesHolidays> Holidays { get; set; }
        [JsonProperty("Airlines")]
        public List<IvsVisaCountriesAirlines> Airlines { get; set; }
        [JsonProperty("Airports")]
        public List<IvsVisaCountriesAirports> Airports { get; set; }

    }
    public class DiplomaticRepresentation
    {
        [JsonProperty("Offices")]
        [XmlElement("Offices")]
        public Office Office;
    }
    public class Office
    {
        [XmlElement("Office")]
        [JsonProperty("Office")]
        public List<IvsVisaCountriesDiplomaticRepresentation> Offices { get; set; }

    }
    public class IntlHelpAddress
    {
        [JsonProperty("HelpAddress")]
        [XmlElement("HelpAddress")]
        public List<IvsVisaHelpAddress> HelpAddress { get; set; }
    }
    public class Descriptions
    {
        [XmlElement("Description")]
        public List<string> Description;
    }
    public class SAARCInfo
    {
        [JsonProperty("CountryOffices")]
        [XmlElement("CountryOffices")]
        public CountryOffice CountryOffice;
    }
    public class CountryOffice
    {
        [XmlElement("CountryOffice")]
        [JsonProperty("CountryOffice")]
        public List<IvsVisaSaarcDetails> CountryOffices { get; set; }

    }
    /*  public class VisaDetails
      {
          [JsonIgnore]
          [System.Xml.Serialization.XmlAttribute()]
          public string CountryCode { get; set; }
          [JsonProperty("CountryName")]
          [Key]
          public List<CountryName> cNames { get; set; }
          [JsonProperty("CountryDetails")]
          public List<IvsVisaCountriesDetails> CountryDetails { get; set; }
          [JsonProperty("Holiydays")]
          public List<IvsVisaCountriesHolidays> Holidays { get; set; }
          [JsonProperty("Airlines")]
          public List<IvsVisaCountriesAirlines> Airlines { get; set; }
          [JsonProperty("Airports")]
          public List<IvsVisaCountriesAirports> Airports { get; set; }
          [JsonProperty("DiplomaticRepresentation")]
          public List<IvsVisaCountriesDiplomaticRepresentation> DiplomaticRepresentation  { get; set; }
          [JsonProperty("IndianEmbassy")]
          public List<IvsVisaHelpAddress> IndianEmbassy { get; set; }
          [JsonProperty("IntlHelpAddress")]
          public List<IvsVisaHelpAddress> IntlHelpAddress { get; set; }
          [JsonProperty("IVSAdvisory")]
          public List<IvsVisaCountryAdvisories> IVSAdvisory { get; set; }
          [JsonProperty("ReciprocalVisaInfo")]
          public List<IvsVisaCountryAdvisories> ReciprocalVisaInfo { get; set; }
          [JsonProperty("InternationalAdvisory")]
          public List<IvsVisaCountryAdvisories> InternationalAdvisory { get; set; }
          [JsonProperty("SAARCInfo")]
          public List<IvsVisaSaarcDetails> SAARCInfo { get; set; }
          [JsonProperty("VisaInformation")]
          public List<IvsVisaInformation> Visainf { get; set; }
          [JsonProperty("Categories")]
          public List<IvsVisaCategories> Categories { get; set; }
          [JsonProperty("CategoryFees")]
          public List<IvsVisaCategoriesOptions> CategoryFees { get; set; }

      }*/
    // models for input data
    /*  public class VisaInputSearchRequest
      {
          public string Id { get; set; }
          public string UserName { get; set; }
          public string Password { get; set; }
          public string CountryCode { get; set; }
      }*/
    //

}
