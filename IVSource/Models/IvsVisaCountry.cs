using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaCountry : MemberPageBase
    {
        public int SerialNum { get; set; }
        public string CountryName { get; set; }
        public string Country { get; set; }
        public string CountryIso { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string CountryLocation { get; set; }
        [Required]
        [Display(Name = "Time")]
        public string CountryTime { get; set; }
        [Required]
        [Display(Name = "Capital")]
        public string CountryCapital { get; set; }
        [Display(Name = "Language")]
        public string CountryLanguages { get; set; }
        [Display(Name = "Area")]
        public string CountryArea { get; set; }
        [Display(Name = "Population")]
        public string CountryPopulation { get; set; }
        [Display(Name = "National Day")]
        public string CountryNationalDay { get; set; }
        [Display(Name = "Currency")]
        public string CountryCurrency { get; set; }
        [Display(Name = "Climate")]
        public string CountryClimate { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string HolidayName { get; set; }

        [Required]
        [Display(Name = "Airline Name")]
        public string AirlineName { get; set; }
        public string AirlineCode { get; set; }

        [Required]
        [Display(Name = "Airport Name")]
        public string AirportName { get; set; }
        public string AirportCode { get; set; }

        public string CountryFlag { get; set; }
        public string CountrySmallMap { get; set; }

        public string CountryLargeMap { get; set; }
    }

    public class IvsVisaCountris : MemberPageBase
    {
        public IvsVisaCountry IvsVisaCountryr { get; set; }
        public List<IvsVisaCountry> AirportName1 { get; set; }
        public List<IvsVisaCountry> HolidayName1 { get; set; }
        public List<IvsVisaCountry> AirlineName1 { get; set; }
        public IvsVisaPages Logo { get; set; }
    }
}
