using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IVSource.Models
{

    public partial class IvsVisaCountriesDetails : MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        public int SerialNum { get; set; }
        [NotMapped]
        //[XmlElement]
        //[JsonProperty]
        //public string CountryName { get; set; }
        public CountryName CountryName { get; set; }
        [Display(Name = "Area")]
        [XmlElement("Area")]
        [JsonProperty("Area")]
        public string CountryArea { get; set; }
        [Required]
        [Display(Name = "Capital")]
        [XmlElement("Capital")]
        [JsonProperty("Capital")]
        public string CountryCapital { get; set; }
        [Display(Name = "Climate")]
        [XmlElement("Climate")]
        [JsonProperty("Climate")]
        public string CountryClimate { get; set; }
        [Display(Name = "Currency")]
        [XmlElement("Currency")]
        [JsonProperty("Currency")]
        public string CountryCurrency { get; set; }
        [Required]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed")]
        [Display(Name = "Flag")]
        [XmlElement("Flag")]
        [JsonProperty("Flag")]
        public string CountryFlag { get; set; }
        [XmlElement("Code")]
        [JsonProperty("Code")]
        public string CountryIso { get; set; }
        [Display(Name = "Language")]
        [XmlElement("Languages")]
        [JsonProperty("Languages")]
        public string CountryLanguages { get; set; }
        [Display(Name = "Large Map")]

        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed")]
        [XmlElement("LargeMap")]
        [JsonProperty("LargeMap")]
        public string CountryLargeMap { get; set; }
        [Required]
        [Display(Name = "Location")]
        [XmlElement("Location")]
        [JsonProperty("Location")]
        public string CountryLocation { get; set; }
        [Display(Name = "National Day")]
        [XmlElement("NationalDay")]
        [JsonProperty("NationalDay")]
        public string CountryNationalDay { get; set; }
        [Display(Name = "Population")]
        [XmlElement("Population")]
        [JsonProperty("Population")]
        public string CountryPopulation { get; set; }
        [Display(Name = "Small Map")]
        [XmlElement("SmallMap")]
        [JsonProperty("SmallMap")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed")]  
        public string CountrySmallMap { get; set; }
        [Required]
        [Display(Name ="Time")]
        [XmlElement("Time")]
        [JsonProperty("Time")]
        public string CountryTime { get; set; }
        [Required]
        [Url]
        [Display(Name = "World Fact Book")]
        [XmlElement("WorldFactBook")]
        [JsonProperty("WorldFactBook")]
        public string CountryWorldFactBook { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }

    }
    //public class CountryName
    //{
    //    [Key]
    //    [XmlElement]
    //    [JsonProperty]
    //    public string Name { get; set; }

    //}
    
}
