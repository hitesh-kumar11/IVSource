using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class HomePage : MemberPageBase
    {
        public IvsVisaPages ContactUS { get; set; }
        public IvsVisaPagesForSubscribe Subscribe { get; set; }
        public IvsVisaPagesForNews News { get; set; }
        public List<IvsVisaSubPages> NewsList { get; set; }

        public IvsVisaPagesForVisitorsArea VisitorsArea { get; set; }

        public List<IvsVisaSubPages> VisitorsList { get; set; }

        public IvsVisaPages Services { get; set; }
        public IvsVisaPagesForPartners Partners { get; set; }

        public List<IvsVisaSubPages> PartnersList { get; set; }

        public IvsVisaPages Logo { get; set; }
        public IvsVisaPages AboutUs { get; set; }
        public IvsVisaPages MEAUsefulLinks { get; set; }
        public IvsVisaPages EVisaIntoIndia { get; set; }
        public IvsVisaPages WhoHealthRulesWorldWide { get; set; }
        public IvsVisaPages FlightsFromCom { get; set; }
        public IvsVisaPages FlightRadar { get; set; }
        public IvsVisaPages PlanYourTravel { get; set; }
        public IvsVisaPages ClickForPresentation { get; set; }

        public IvsVisaPagesLinks Links { get; set; }

        public IvsVisaPagesOurContent Pages { get; set; }
        public List<IvsVisaPages> PagesOurContent { get; set; }

        public List<IvsVisaPages> PagesOurContentPagesList { get; set; }
        public List<IvsVisaPages> PagesOurContentLinksList { get; set; }

        public List<IvsVisaSubPages> SubPagesNews { get; set; }
        public Login login { get; set; }
        public IvsVisaCountris countryfactfinder { get; set; }

        public IvsVisaCountry countryfactfinderr { get; set; }

        public List<IvsVisaCountry> AirportName1 { get; set; }

        public List<IvsVisaCountry> HolidayName1 { get; set; }
        public List<IvsVisaCountry> AirlineName1 { get; set; }
        public List<IvsVisaCountriesDiplomaticRepresentation> diplomaticrepresentation { get; set; }
        public string CityName { get; set; }

        public List<IvsVisaNoteFees> CityDetails { get; set; }
        public List<IvsVisaNoteFees> CityDetailsInformation { get; set; }
        public List<IvsVisaNoteFees> CityCategoryDetails { get; set; }
        public List<IvsVisaNoteFees> CityCategoryFees { get; set; }
        public List<IvsVisaNoteFees> CityFormDetails { get; set; }
        public List<IvsVisaHelpAddress> helpaddress { get; set; }
        public List<IvsVisaCountryAdvisories> reciprocalvisa { get; set; }
        public List<IvsVisaCountryAdvisories> threeadvisories { get; set; }
        public List<IvsVisaSaarcDetails> saarcDetails { get; set; } 
        public List<IvsVisaCategoriesForms> downloadvisaform { get; set; }
        public List<IvsVisaApi> covidinformation { get; set; }

        public IvsVisaSubPages IvsVisaSubPagesr { get; set; }

        public List<IvsVisaQuickContactInfo> QuickContactInfo { get; set; }
        public List<IvsVisaCountries> IvsVisaCountries { get; set; }

        public IvsVisaPages logo { get; set; }

        public int Priority { get; set; }

        public string AirportNameCode { get; set; }


    }
    public class IvsVisaPagesForSubscribe
    {
        public IvsVisaPages SubscribeToUs { get; set; }
        public IvsVisaSubscribeToUs SubscribeToUsForm { get; set; }
        
    }

    public class IvsVisaPagesForNews
    {
        public IvsVisaPages pages { get; set; }
        public List<IvsVisaSubPages> subPages { get; set; }
    }

    public class IvsVisaPagesForVisitorsArea
    {
        public IvsVisaPages visitorsArea { get; set; }
        public List<IvsVisaSubPages> visitorsSubArea { get; set; }
    }

    public class IvsVisaPagesForPartners
    {
        public IvsVisaPages partners { get; set; }
        public List<IvsVisaSubPages> partnersSubArea { get; set; }
    }

    public class IvsVisaNoteAndFeesListHomePage : MemberPageBase{
        public List<IvsVisaNoteFees> CityDetails { get; set; }
        public List<IvsVisaNoteFees> CityDetailsInformation { get; set; }
        public List<IvsVisaNoteFees> CityCategoryDetails { get; set; }
        public List<IvsVisaNoteFees> CityCategoryFees { get; set; }
        public List<IvsVisaNoteFees> CityFormDetails { get; set; }
        public string CityName { get; set; }
        public string CityId { get; set; }
    }
}
