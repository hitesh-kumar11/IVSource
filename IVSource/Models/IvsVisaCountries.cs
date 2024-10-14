using Microsoft.AspNetCore.WebSockets.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IVSource.Models
{
    [XmlType(TypeName = "Country")]
    //[DisplayName("Country")]
    public partial class IvsVisaCountries : MemberPageBase 
    {
        [XmlIgnore]
        [JsonIgnore]
        public int SerialNum { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$", ErrorMessage = "The field starts with capital letter. Numbers or any special characters are not allowed.")]
        [StringLength(60)]
        [DisplayName("Country Name")]
        [XmlElement("Name")]
        [JsonProperty("Name")]
        public string CountryName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "The field do not allow numbers or any special character and only two letters are allowed.")]
        [MaxLength(2)]
        [MinLength(2)]
        [DisplayName("Country Iso")]
        [XmlElement("Code")]
        [JsonProperty("Code")]
        public string CountryIso { get; set; }

        [StringLength(60)]
        [DisplayName("Country")]
        [XmlElement("Name")]
        [JsonProperty("Name")]
        public string Country { get; set; }

        [DisplayName("CreatedDate Date")]
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }

        [DisplayName("Modified Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/dd/mm}")]
        [XmlElement("UpdatedOn")]
        [JsonProperty("UpdatedOn")]
        public DateTime? ModifiedDate { get; set; }

        [Required]
        [DisplayName("Is Enabled")]
        [XmlIgnore]
        [JsonIgnore]
        public int? IsEnable { get; set; }

        [Required]
        [DisplayName("Is Updated")]
        [XmlIgnore]
        [JsonIgnore]
        public int IsUpdated { get; set; }
    }

    public class CountryName
    {
        [Key]
        [XmlElement]
        [JsonProperty]
        public string Name { get; set; }

    }
}
