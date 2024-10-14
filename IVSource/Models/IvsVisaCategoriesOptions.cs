using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IVSource.Models
{
    //Add NullValueHandling code by Hitesh on 17-05-2023
    public partial class IvsVisaCategoriesOptions
    {
        [XmlIgnore]
        [JsonIgnore]
        public int SerialNum { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryOptionId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CountryIso { get; set; }
        [Display(Name = "Visa Category Code")]
        [XmlElement("CategoryCode")]
        [JsonProperty("CategoryCode", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryCode { get; set; }
        [Required]
        [Display(Name = "Category Option")]
        [XmlElement("Category")]
        [JsonProperty("Category", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryOption { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string VisaCategoryOptionCode { get; set; }
        [Display(Name = "Visa Category Option Fee (INR)")]
        [XmlElement("CategoryFeeAmountINR")]
        [JsonProperty("CategoryFeeAmountINR", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryOptionAmountInr { get; set; }
        [Display(Name = "Visa Category Option Fee (Non INR)")]
        [XmlElement("CategoryFeeAmountOther")]
        [JsonProperty("CategoryFeeAmountOther", NullValueHandling = NullValueHandling.Ignore)]
        public string VisaCategoryOptionAmountOther { get; set; }
        [Display(Name = "Enable")]
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ModifiedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [NotMapped]
        [Display(Name = "City Name")]
        public string CityName { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [NotMapped]
        [Display(Name = "Category Name")]
        public string VisaCategory { get; set; }
        [NotMapped]
        public List<IvsVisaCategories> VisaCategoryList { get; set; }
    }
}
