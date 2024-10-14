using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IVSource.Models
{
    //Add NullValueHandling code by Hitesh on 17-05-2023
    public partial class IvsVisaCategories : MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CountryIso { get; set; }

        [Required]
        [Display(Name = "Visa Category Name")]
        [XmlElement("Category")]
        [JsonProperty("Category", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategory { get; set; }

        [Required]
        [Display(Name = "Visa Category Code")]
        [XmlElement("CategoryCode")]
        [JsonProperty("CategoryCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryId { get; set; }

        [Required]
        [Display(Name = "Visa Category Information")]
        [XmlElement("CategoryInfo")]
        [JsonProperty("CategoryInfo", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryInformation { get; set; }

        [Display(Name = "Visa Category Notes")]
        [XmlElement("CategoryNotes")]
        [JsonProperty("CategoryNotes", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryNotes { get; set; }

        [Display(Name = "Visa Category Requirements")]
        [XmlElement("CategoryRequirements")]
        [JsonProperty("CategoryRequirements", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryRequirements { get; set; }

        [Required]
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Territory City")]
        public string CityId { get; set; }

        [Display(Name = "Visa Category Code")]
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryCode { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Enable")]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Priority { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "Visa Category Information Visa Procedure")]
        [XmlElement("CategoryInformationVisaProcedure")]
        [JsonProperty("CategoryInformationVisaProcedure")]
        public string VisaCategoryInformationVisaProcedure { get; set; }

        [Display(Name = "Visa Category Information Documents Required")]
        [XmlElement("CategoryInformationDocumentsRequired")]
        [JsonProperty("CatgeoryInformationDocumentsRequired")]
        public string VisaCategoryInformationDocumentsRequired { get; set; }

        [Display(Name = "Visa Category Information Processing Time")]
        [XmlElement("CategoryInformationProcessingTime")]
        [JsonProperty("CategoryInformationProcessingTime")]
        public string VisaCategoryInformationProcessingTime { get; set; }
        [NotMapped]
        [Display(Name = "Visa Category City")]
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityName { get; set; }
        [NotMapped]
        public List<IvsVisaCategoriesForApi2> IvsVisaCategoriesForApi2 { get; set; }

        [NotMapped]
        [XmlElement("CategoryFeesDetails")]
        public CategoryFeeDetails CategoryFeesDetails { get; set; }

        [NotMapped]
        public List<IvsVisaCategoriesOptions> CategoryFees { get; set; }
    }

    public class CategoryFeeDetails
    {
        public List<IvsVisaCategoriesOptions> CategoryFee { get; set; }
    }
    public class IvsVisaCategories1
    {
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CountryIso { get; set; }

        [Required]
        [Display(Name = "Visa Category Name")]
        [XmlElement("Category")]
        [JsonProperty("Category", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategory { get; set; }

        [Required]
        [Display(Name = "Visa Category Code")]
        [XmlElement("CategoryCode")]
        [JsonProperty("CategoryCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryId { get; set; }  
       
        [Required]
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Territory City")]
        public string CityId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Display(Name = "Enable")]
        public int? IsEnable { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Priority { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedDate { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        [Display(Name = "Visa Category City")]
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityName { get; set; }       
    }
    public class IvsVisaCategoriesForApi2
    {
        [XmlElement("Category")]
         public IvsVisaCategories Categories { get; set; }
    }
}
