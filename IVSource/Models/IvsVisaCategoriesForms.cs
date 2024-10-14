using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaCategoriesForms: MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        public int SerialNum { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CountryIso { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Territory City")]

        //public string CityValue { get; set; }
        public string CityId { get; set; }
        //[XmlIgnore]
        //[JsonIgnore]
        public string FormId { get; set; }
        [XmlElement("CategoryCode")]
        [JsonProperty("CategoryCode")]
        [Display(Name = "Category Code")]
        public string VisaCategoryCode { get; set; }
        [XmlElement("Form")]
        [JsonProperty("Form")]
        [Required]
        [Display(Name = "Form Name")]
        public string Form { get; set; }
        [XmlElement("FormPath")]
        [JsonProperty("FormPath")]
        [Display(Name = "Category Form Path")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$", ErrorMessage = "Only Pdf files allowed")]
        public string FormPath { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Enable")]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
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

        //[Required]
        //public List<IvsVisaCategories> Categories { get; set; }

        //[Required]
        //public IvsVisaCategoriesForms categoriesForms { get; set; }

    }
}
