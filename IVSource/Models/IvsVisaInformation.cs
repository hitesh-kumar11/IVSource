using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace IVSource.Models
{
    public partial class IvsVisaInformation : MemberPageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CountryIso { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CityId { get; set; }
        [NotMapped]
        [XmlElement("TerritoryCity")]
        [JsonProperty("TerritoryCity")]
        public string CityName { get; set; }
        [Display(Name = "Visa General Information")]
        [XmlIgnore]
        public string VisaGeneralInformation { get; set; }
        [XmlElement("VisaInfo")]
        [JsonProperty("VisaInfo")]
        [NotMapped]
        public IvsVisaCategoriesForApi1 VisaGeneralInformation1 { get; set; }
        [NotMapped]
        public IvsVisaCategoriesForApi2 IvsVisaCategoriesForApi2 { get; set; }
        [XmlIgnore]
        [Required]
        [Display(Name = "Visa Information")]
        [XmlElement("VisaInformation")]
        [JsonProperty("VisaInformation")]
        public string VisaInformation { get; set; }

        [Required]
        [Display(Name = "Enable")]
        [XmlIgnore]
        [JsonIgnore]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string Priority { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Changed by Hitesh on 29082023_1142 as required format by Vinay #1 Step 1
        /// </summary>

        [NotMapped]
        [XmlElement("Categories")]
        public IvsVisaInformationCategories Categories { get; set; }
        [NotMapped]
        [XmlElement("CategoryFees")]
        public List<IvsVisaCategoriesFees> CategoryFees { get; set; }
        //
        [NotMapped]
        [XmlElement("CategoryForms")]
        public List<CategoryForms> IvsVisaCategoryForms { get; set; }


        /// <summary>
        /// #1
        /// </summary>
        /// 

        // for visacatid for getting catfees
        [NotMapped]
        public Categorydetails categorydetails { get; set; }
        //
    }
    public class IvsVisaInformationCategories
    {
        [XmlElement("Category")]
        public List<IvsVisaCategories> Category { get; set; }
    }
    //public class IvsVisaCategoriesFees
    //{
    //    [XmlElement("CategoryFees")]
    //    public List<IvsVisaCategoriesFee> CategoryFees { get; set; }
    //}
    //public class IvsVisaCategoriesFee
    //{
    //    [XmlElement("CategoryFee")]
    //    public List<IvsVisaCategoriesOptions> CategoryFee { get; set; }
    //}

    public class IvsVisaCategoriesFees
    {
        [XmlElement("CategoryFee")]
        public List<IvsVisaCategoriesOptions> CategoryFee { get; set; }
    }

    //public class CategoryForms
    //{
    //    [XmlElement("CategoryForms")]
    //    public List<VisaCategoryForms> CategoryFormList { get; set; }
    //}
    //public class VisaCategoryForms
    //{
    //    [XmlElement("CategoryForm")]
    //    public List<IvsVisaCategoriesForms> VisaCategoryForm { get; set; }
    //}

    public class CategoryForms
    {
        [XmlElement("CategoryForm")]
        public List<IvsVisaCategoriesForms> CategoryForm { get; set; }
    }
    public class IvsVisaCategoriesForApi1
    {
        [XmlElement("VisaGeneralInformation")]
        public string VisaCategoryRequirements { get; set; }
        [XmlElement("VisaInformation")]
        public string VisaInformation { get; set; }


        //Added by Hitesh on 29082023_1145 Step 2

        [NotMapped]
        [XmlElement("Categories")]
        public IvsVisaInformationCategories Categories { get; set; }
        [NotMapped]
        [XmlElement("CategoryFees")]
        public IvsVisaCategoriesFees CategoryFees { get; set; }
        //
        [NotMapped]
        [XmlElement("CategoryForms")]
        public CategoryForms CategoryForms { get; set; }

    }
    // class for get visacategoryid
    public class Categorydetails
    {
        //public List<IvsVisaCategories> IvsVisaCategories1 { get; set; }
        //public List<IvsVisaCategoriesOptions> IvsVisaCategoriesOptions1 { get; set; }
        [XmlElement("VisaGeneralInformation")]
        public string VisaCategoryRequirements { get; set; }
        [XmlElement("VisaInformation")]
        public string VisaInformation { get; set; }
        [XmlElement("Category")]
        [JsonProperty("Category")]
        public string VisaCategory { get; set; }
        [XmlElement("CategoryCode")]
        [JsonProperty("CategoryCode")]
        public string VisaCategoryId { get; set; }
        [XmlElement("CategoryFeeAmountINR")]
        [JsonProperty("CategoryFeeAmountINR")]
        public string VisaCategoryOptionAmountInr { get; set; }
        [XmlElement("CategoryFeeAmountOther")]
        [JsonProperty("CategoryFeeAmountOther")]
        public string VisaCategoryOptionAmountOther { get; set; }
    }

    public class IvsVisaInformation1 
    {
        [XmlIgnore]
        [JsonIgnore]
        [Display(Name = "Serial No.")]
        public int SerialNum { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CountryIso { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string CityId { get; set; }
        [NotMapped]
        [XmlElement("TerritoryCity")]
        [JsonProperty("TerritoryCity")]
        public string CityName { get; set; } 
        [Required]
        [Display(Name = "Enable")]
        [XmlIgnore]
        [JsonIgnore]
        public int? IsEnable { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public int Priority { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public DateTime? ModifiedDate { get; set; }        
    }
}
