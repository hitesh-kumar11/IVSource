using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using IVSource.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IVSource.Models
{
  public partial class IVSourceContext : DbContext
  {
    public IVSourceContext()
    {
    }
    public IVSourceContext(DbContextOptions<IVSourceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<IvsCountryTravelInfo> IvsCountryTravelInfo { get; set; }
    //public virtual DbSet<IvsCountryTravelTourism> IvsCountryTravelTourism { get; set; }
    //public virtual DbSet<IvsTravelCategories> IvsTravelCategories { get; set; }
    public virtual DbSet<IvsUserDetails> IvsUserDetails { get; set; }
    public virtual DbSet<IvsUserTerminalId> IvsUserTerminalId { get; set; }
    public virtual DbSet<IvsVisaApi> IvsVisaApi { get; set; }
    public virtual DbSet<IvsVisaCategories> IvsVisaCategories { get; set; }
    public virtual DbSet<IvsVisaCategoriesCity> IvsVisaCategoriesCity { get; set; }
    public virtual DbSet<IvsVisaCategoriesForms> IvsVisaCategoriesForms { get; set; }
    public virtual DbSet<IvsVisaCategoriesOptions> IvsVisaCategoriesOptions { get; set; }
    public virtual DbSet<IvsVisaClientIpRegistration> IvsVisaClientIpRegistration { get; set; }
    public virtual DbSet<IvsVisaCompanyMaster> IvsVisaCompanyMaster { get; set; }
    public virtual DbSet<IvsVisaCountries> IvsVisaCountries { get; set; }
    public virtual DbSet<IvsVisaCountriesAirlines> IvsVisaCountriesAirlines { get; set; }
    public virtual DbSet<IvsVisaCountriesAirports> IvsVisaCountriesAirports { get; set; }
    public virtual DbSet<IvsVisaCountriesAlias> IvsVisaCountriesAlias { get; set; }
    public virtual DbSet<IvsVisaCountriesDetails> IvsVisaCountriesDetails { get; set; }
    public virtual DbSet<IvsVisaCountriesDiplomaticRepresentation> IvsVisaCountriesDiplomaticRepresentation { get; set; }
    public virtual DbSet<IvsVisaCountriesHolidays> IvsVisaCountriesHolidays { get; set; }
    //public virtual DbSet<IvsVisaCountry> IvsVisaCountry { get; set; }
    public virtual DbSet<IvsVisaCountryAdvisories> IvsVisaCountryAdvisories { get; set; }
    public virtual DbSet<IvsVisaCountryTerritoryCities> IvsVisaCountryTerritoryCities { get; set; }
    public virtual DbSet<IvsVisaHelpAddress> IvsVisaHelpAddress { get; set; }
    public virtual DbSet<IvsVisaInformation> IvsVisaInformation { get; set; }
    public virtual DbSet<IvsVisaMenus> IvsVisaMenus { get; set; }
    public virtual DbSet<IvsVisaPages> IvsVisaPages { get; set; }
    public virtual DbSet<IvsVisaSaarcDetails> IvsVisaSaarcDetails { get; set; }
    public virtual DbSet<IvsVisaSubPages> IvsVisaSubPages { get; set; }
    public virtual DbSet<IvsVisaQuickContactInfo> IvsVisaQuickContactInfo { get; set; }

        // Unable to generate entity type for table 'ivsource.wp_cntctfrm_field'. Please see the warning messages.

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //      if (!optionsBuilder.IsConfigured)
        //      {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //        optionsBuilder.UseSqlServer("Data Source=ITQ-UATSQL-SRV;Initial Catalog=IVSourceUAT;Persist Security Info=True;User ID=SCRIPU;Password=itq!@#$1234");
        //      }
        //    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
      modelBuilder.Entity<IvsCountryTravelInfo>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_country_travel_info", "ivsource");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.TravelCategory)
            .HasColumnName("travel_category")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.TravelDescription)
            .HasColumnName("travel_description")
            .IsUnicode(false);

        entity.Property(e => e.TravelInfoId)
            .HasColumnName("travel_info_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.TravelType)
            .HasColumnName("travel_type")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      //modelBuilder.Entity<IvsCountryTravelTourism>(entity =>
      //{
      //    entity.HasKey(e => e.TravelTypeId);

      //    entity.ToTable("ivs_country_travel_tourism", "ivsource");

      //    entity.HasIndex(e => e.CountryIso)
      //        .HasName("country_iso_idx");

      //    entity.HasIndex(e => e.SerialNum)
      //        .HasName("serial_num_idx");

      //    entity.HasIndex(e => e.TravelTourismId)
      //        .HasName("travel_tourism_id_idx");

      //    entity.Property(e => e.TravelTypeId)
      //        .HasColumnName("travel_type_id")
      //        .HasMaxLength(20)
      //        .IsUnicode(false)
      //        .HasDefaultValueSql("(N'')");

      //    entity.Property(e => e.CountryIso)
      //        .HasColumnName("country_iso")
      //        .HasMaxLength(10)
      //        .IsUnicode(false)
      //        .HasDefaultValueSql("(N'')");

      //    entity.Property(e => e.CreatedDate)
      //        .HasColumnName("created_date")
      //        .HasColumnType("datetime")
      //        .HasDefaultValueSql("(getdate())");

      //    entity.Property(e => e.IsEnable)
      //        .HasColumnName("is_enable")
      //        .HasDefaultValueSql("((0))");

      //    entity.Property(e => e.ModifiedDate)
      //        .HasColumnName("modified_date")
      //        .HasColumnType("datetime")
      //        .HasDefaultValueSql("(getdate())");

      //    entity.Property(e => e.SerialNum)
      //        .HasColumnName("serial_num")
      //        .ValueGeneratedOnAdd();

      //    entity.Property(e => e.TravelDescription)
      //        .HasColumnName("travel_description")
      //        .IsUnicode(false);

      //    entity.Property(e => e.TravelTourismId)
      //        .HasColumnName("travel_tourism_id")
      //        .HasMaxLength(20)
      //        .IsUnicode(false)
      //        .HasDefaultValueSql("(N'')");
      //});

      //modelBuilder.Entity<IvsTravelCategories>(entity =>
      //{
      //    entity.HasKey(e => e.CategoryId);

      //    entity.ToTable("ivs_travel_categories", "ivsource");

      //    entity.HasIndex(e => e.SerialNum)
      //        .HasName("serial_num_idx");

      //    entity.Property(e => e.CategoryId)
      //        .HasColumnName("category_id")
      //        .HasMaxLength(20)
      //        .IsUnicode(false)
      //        .HasDefaultValueSql("(N'')");

      //    entity.Property(e => e.CreatedDate)
      //        .HasColumnName("created_date")
      //        .HasColumnType("datetime")
      //        .HasDefaultValueSql("(getdate())");

      //    entity.Property(e => e.IsEnable)
      //        .HasColumnName("is_enable")
      //        .HasDefaultValueSql("((0))");

      //    entity.Property(e => e.ModifiedDate)
      //        .HasColumnName("modified_date")
      //        .HasColumnType("datetime")
      //        .HasDefaultValueSql("(getdate())");

      //    entity.Property(e => e.SerialNum)
      //        .HasColumnName("serial_num")
      //        .ValueGeneratedOnAdd();

      //    entity.Property(e => e.TravelCategory)
      //        .HasColumnName("travel_category")
      //        .HasMaxLength(250)
      //        .IsUnicode(false)
      //        .HasDefaultValueSql("(N'')");

      //    entity.Property(e => e.TravelType)
      //        .HasColumnName("travel_type")
      //        .HasMaxLength(150)
      //        .IsUnicode(false)
      //        .HasDefaultValueSql("(N'')");
      //});

      modelBuilder.Entity<IvsUserDetails>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_user_details", "ivsource");

        entity.HasIndex(e => e.IsEnable)
            .HasName("idx_is_enable");

        entity.HasIndex(e => e.Password)
            .HasName("idx_password");

        entity.HasIndex(e => e.UserName)
            .HasName("idx_user_name");

        entity.HasIndex(e => e.UserType)
            .HasName("idx_user_type");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.AdditionalEmail).HasColumnName("additional_email");

        entity.Property(e => e.Address).HasColumnName("address");

        entity.Property(e => e.City)
            .HasColumnName("city")
            .HasMaxLength(200)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.Company)
            .HasColumnName("company")
            .HasMaxLength(250);

        entity.Property(e => e.CorporateId)
            .HasColumnName("corporate_id")
            .HasMaxLength(50)
            .IsUnicode(false);

        entity.Property(e => e.Country)
            .HasColumnName("country")
            .HasMaxLength(200)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Designation)
            .HasColumnName("designation")
            .HasMaxLength(250);

        entity.Property(e => e.Email).HasColumnName("email");

        entity.Property(e => e.Fax)
            .HasColumnName("fax")
            .HasMaxLength(100)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.Password)
            .HasColumnName("password")
            .HasMaxLength(100)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.Phone)
            .HasColumnName("phone")
            .HasMaxLength(100)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.ResetPasswordOtp)
            .HasColumnName("ResetPasswordOTP")
            .HasMaxLength(10)
            .IsUnicode(false);

        entity.Property(e => e.ResetPasswordOtpexpireOn)
            .HasColumnName("ResetPasswordOTPExpireOn")
            .HasColumnType("smalldatetime");

        entity.Property(e => e.TerminalIdNo)
            .HasColumnName("terminal_id_no")
            .HasMaxLength(10);

        entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(15)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.UserName)
            .HasColumnName("user_name")
            .HasMaxLength(100)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.UserType)
            .HasColumnName("user_type")
            .HasMaxLength(20)
            .HasDefaultValueSql("(N'USER')");

        entity.Property(e => e.ValidFrom)
            .HasColumnName("valid_from")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.ValidTo)
            .HasColumnName("valid_to")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");
      });

      modelBuilder.Entity<IvsUserTerminalId>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_user_terminal_id", "ivsource");

        entity.HasIndex(e => e.TerminalId)
            .HasName("idx_terminal_id");

        entity.HasIndex(e => e.UserId)
            .HasName("idx_user_id");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsUsed)
            .HasColumnName("is_used")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.TerminalId)
            .HasColumnName("terminal_id")
            .HasMaxLength(250)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
          entity.Property(e => e.SessionID)
             .HasColumnName("Session_ID")
             .HasMaxLength(250)
             .IsUnicode(false)
             .HasDefaultValueSql("(N'')");

          entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      modelBuilder.Entity<IvsVisaApi>(entity =>
      {
        entity.ToTable("IVS_VISA_API");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        entity.Property(e => e.Password)
            .HasMaxLength(50)
            .IsUnicode(false);

        entity.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(10)
            .IsUnicode(false);

        entity.Property(e => e.UpdatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.Url)
            .IsRequired()
            .HasColumnName("URL")
            .HasMaxLength(200)
            .IsUnicode(false);

        entity.Property(e => e.Username)
            .HasMaxLength(50)
            .IsUnicode(false);
      });


            modelBuilder.Entity<IvsVisaCategoriesCity>(entity =>
            {
                entity.HasKey(e => e.city); entity.ToTable("IVS_VISA_CATEGORIES_City", "ivsource");
                entity.HasKey(e => e.city_code).HasName("city_code");
                entity.Property(e => e.is_enable).HasColumnName("is_enable").HasDefaultValueSql("((0))");
            });




            modelBuilder.Entity<IvsVisaCategories>(entity =>
            {
                entity.HasKey(e => e.VisaCategoryId);  

          entity.ToTable("ivs_visa_categories", "ivsource");

                entity.HasIndex(e => e.CityId)  
              .HasName("city_id_idx");

                entity.HasIndex(e => e.CountryIso) 
              .HasName("country_iso_idx");

                entity.HasIndex(e => e.SerialNum)  
              .HasName("serial_num_idx");

                entity.Property(e => e.VisaCategoryId) 
              .HasColumnName("visa_category_id")
              .HasMaxLength(20)
              .IsUnicode(false)
              .HasDefaultValueSql("(N'')");

                entity.Property(e => e.CityId) 
              .HasColumnName("city_id")
              .HasMaxLength(20)
              .IsUnicode(false)
              .HasDefaultValueSql("(N'')");

                entity.Property(e => e.CountryIso) 
              .IsRequired()
              .HasColumnName("country_iso")
              .HasMaxLength(10)
              .IsUnicode(false)
              .HasDefaultValueSql("(N'')");

                entity.Property(e => e.CreatedDate)   
              .HasColumnName("created_date")
              .HasColumnType("datetime")
              .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsEnable)  
              .HasColumnName("is_enable")
              .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedDate)  
              .HasColumnName("modified_date")
              .HasColumnType("datetime")
              .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Priority) 
              .HasColumnName("priority")
              .HasMaxLength(100)
              .IsUnicode(false)
              .HasDefaultValueSql("(N'100')");

                entity.Property(e => e.SerialNum)  
              .HasColumnName("serial_num")
              .ValueGeneratedOnAdd();

                entity.Property(e => e.VisaCategory) 
              .HasColumnName("visa_category")
              .HasMaxLength(200)
              .IsUnicode(false)
              .HasDefaultValueSql("(N'')");

                entity.Property(e => e.VisaCategoryCode)  
              .HasColumnName("visa_category_code")
              .HasMaxLength(10)
              .IsUnicode(false)
              .HasDefaultValueSql("(N'')");

                entity.Property(e => e.VisaCategoryInformation) 
              .HasColumnName("visa_category_information")
              .IsUnicode(false);

                entity.Property(e => e.VisaCategoryInformationVisaProcedure) 
              .HasColumnName("visa_category_information_visa_procedure")
              .IsUnicode(false);


                entity.Property(e => e.VisaCategoryInformationDocumentsRequired)  
              .HasColumnName("visa_category_information_documents_required")
              .IsUnicode(false);

                entity.Property(e => e.VisaCategoryInformationProcessingTime)  
              .HasColumnName("visa_category_information_processing_time")
              .IsUnicode(false);


                entity.Property(e => e.VisaCategoryNotes) 
              .HasColumnName("visa_category_notes")
              .IsUnicode(false);

                entity.Property(e => e.VisaCategoryRequirements) 
              .HasColumnName("visa_category_requirements")
              .IsUnicode(false);
            });

            modelBuilder.Entity<IvsVisaCategoriesForms>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_categories_forms", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.FormId)
            .HasName("ivs_visa_categories_forms$form_id")
            .IsUnique();

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CityId)
            .HasColumnName("city_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .IsRequired()
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Form)
            .HasColumnName("form")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.FormId)
            .HasColumnName("form_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.FormPath)
            .HasColumnName("form_path")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.VisaCategoryCode)
            .HasColumnName("visa_category_code")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      modelBuilder.Entity<IvsVisaCategoriesOptions>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_categories_options", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.IsEnable)
            .HasName("is_enable_4");

        entity.HasIndex(e => e.VisaCategoryOptionId)
            .HasName("visa_category_option_id");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CountryIso)
            .IsRequired()
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.VisaCategoryCode)
            .HasColumnName("visa_category_code")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaCategoryOption)
            .HasColumnName("visa_category_option")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaCategoryOptionAmountInr)
            .HasColumnName("visa_category_option_amount_INR")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaCategoryOptionAmountOther)
            .HasColumnName("visa_category_option_amount_other")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaCategoryOptionCode)
            .HasColumnName("visa_category_option_code")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaCategoryOptionId)
            .IsRequired()
            .HasColumnName("visa_category_option_id")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      modelBuilder.Entity<IvsVisaClientIpRegistration>(entity =>
      {
        entity.ToTable("IVS_VISA_Client_IP_Registration");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(20)
            .IsUnicode(false);

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.UpdatedDate).HasColumnType("smalldatetime");
      });

      modelBuilder.Entity<IvsVisaCompanyMaster>(entity =>
      {
        entity.ToTable("IVS_VISA_COMPANY_MASTER");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.CompanyDescription)
            .HasMaxLength(1000)
            .IsUnicode(false);

        entity.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(500)
            .IsUnicode(false);

        entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.UpdatedDate).HasColumnType("smalldatetime");
      });

      modelBuilder.Entity<IvsVisaCountries>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_countries", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("ivs_visa_countries$country_iso")
            .IsUnique();

        entity.HasIndex(e => e.IsEnable)
            .HasName("idx_is_enable");

        entity.HasIndex(e => e.IsUpdated)
            .HasName("is_updated_idx");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryName)
            .IsRequired()
            .HasColumnName("country_name")
            .HasMaxLength(100)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.IsUpdated).HasColumnName("is_updated");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");
      });

      modelBuilder.Entity<IvsVisaCountriesAirlines>(entity =>
      {
        entity.HasKey(e => e.AirlineId);

        entity.ToTable("ivs_visa_countries_airlines", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.SerialNum)
            .HasName("serial_num_idx");

        entity.Property(e => e.AirlineId)
            .HasColumnName("airline_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.AirlineCode)
            .HasColumnName("airline_code")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.AirlineName)
            .HasColumnName("airline_name")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.SerialNum)
            .HasColumnName("serial_num")
            .ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<IvsVisaCountriesAirports>(entity =>
      {
        entity.HasKey(e => e.AirportId);

        entity.ToTable("ivs_visa_countries_airports", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.SerialNum)
            .HasName("serial_num_idx");

        entity.Property(e => e.AirportId)
            .HasColumnName("airport_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.AirportCode)
            .HasColumnName("airport_code")
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.AirportName)
            .HasColumnName("airport_name")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.AirportType)
            .HasColumnName("airport_type")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.SerialNum)
            .HasColumnName("serial_num")
            .ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<IvsVisaCountriesAlias>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_countries_alias", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("ivs_visa_countries_alias$country_iso")
            .IsUnique();

        entity.HasIndex(e => e.IsEnable)
            .HasName("idx_is_enable");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryName)
            .IsRequired()
            .HasColumnName("country_name")
            .HasMaxLength(100)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");
      });

      modelBuilder.Entity<IvsVisaCountriesDetails>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_countries_details", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CountryArea)
            .HasColumnName("country_area")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryCapital)
            .HasColumnName("country_capital")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryClimate)
            .HasColumnName("country_climate")
            .IsUnicode(false);

        entity.Property(e => e.CountryCurrency)
            .HasColumnName("country_currency")
            .IsUnicode(false);

        entity.Property(e => e.CountryFlag)
            .IsRequired()
            .HasColumnName("country_flag")
            .IsUnicode(false);

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryLanguages)
            .HasColumnName("country_languages")
            .IsUnicode(false);

        entity.Property(e => e.CountryLargeMap)
            .HasColumnName("country_large_map")
            .IsUnicode(false);

        entity.Property(e => e.CountryLocation)
            .IsRequired()
            .HasColumnName("country_location")
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryNationalDay)
            .HasColumnName("country_national_day")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryPopulation)
            .HasColumnName("country_population")
            .IsUnicode(false);

        entity.Property(e => e.CountrySmallMap)
            .HasColumnName("country_small_map")
            .IsUnicode(false);

        entity.Property(e => e.CountryTime)
            .HasColumnName("country_time")
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryWorldFactBook)
            .HasColumnName("country_world_fact_book")
            .IsUnicode(false);

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");
      });

      modelBuilder.Entity<IvsVisaCountriesDiplomaticRepresentation>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_countries_diplomatic_representation", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.IsEnable)
            .HasName("is_enable");

        entity.HasIndex(e => e.OfficeId)
            .HasName("ivs_visa_countries_diplomatic_representation$office_id")
            .IsUnique();

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.OfficeAddress)
            .HasColumnName("office_address")
            .IsUnicode(false);

        entity.Property(e => e.OfficeCity)
            .HasColumnName("office_city")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeCollectionTimings)
            .HasColumnName("office_collection_timings")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeCountry)
            .HasColumnName("office_country")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeEmail)
            .HasColumnName("office_email")
            .IsUnicode(false);

        entity.Property(e => e.OfficeFax)
            .HasColumnName("office_fax")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeId)
            .IsRequired()
            .HasColumnName("office_id")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeName)
            .IsRequired()
            .HasColumnName("office_name")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeNotes)
            .HasColumnName("office_notes")
            .IsUnicode(false);

        entity.Property(e => e.OfficePhone)
            .HasColumnName("office_phone")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficePincode)
            .HasColumnName("office_pincode")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficePublicTimings)
            .HasColumnName("office_public_timings")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeTelephoneVisa)
            .HasColumnName("office_telephone_visa")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeTimings)
            .HasColumnName("office_timings")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeVisaTimings)
            .HasColumnName("office_visa_timings")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeWebsite)
            .HasColumnName("office_website")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.Priority)
            .HasColumnName("priority")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'100')");

        entity.Property(e => e.TerritoryJurisdiction)
            .HasColumnName("territory_jurisdiction")
            .IsUnicode(false);
      });

      modelBuilder.Entity<IvsVisaCountriesHolidays>(entity =>
      {
        entity.HasKey(e => e.HolidayId);

        entity.ToTable("ivs_visa_countries_holidays", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.SerialNum)
            .HasName("serial_num_idx");

        entity.Property(e => e.HolidayId)
            .HasColumnName("holiday_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Date)
            .HasColumnName("date")
            .HasMaxLength(2)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.HolidayName)
            .HasColumnName("holiday_name")
            .HasMaxLength(1000)
            .IsUnicode(false);

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Month)
            .HasColumnName("month")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.SerialNum)
            .HasColumnName("serial_num")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Year)
            .HasColumnName("year")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      modelBuilder.Entity<IvsVisaCountryAdvisories>(entity =>
      {
        entity.HasKey(e => e.AdvisoryId);

        entity.ToTable("ivs_visa_country_advisories", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.SerialNum)
            .HasName("serial_num_idx");

        entity.Property(e => e.AdvisoryId)
            .HasColumnName("advisory_id")
            .HasMaxLength(20)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.Advisory)
            .HasColumnName("advisory")
            .IsUnicode(false);

        entity.Property(e => e.AdvisoryType)
            .HasColumnName("advisory_type")
            .HasMaxLength(10)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.SerialNum)
            .HasColumnName("serial_num")
            .ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<IvsVisaCountryTerritoryCities>(entity =>
      {
        entity.HasKey(e => e.CityId);

        entity.ToTable("ivs_visa_country_territory_cities", "ivsource");

        entity.HasIndex(e => e.CityIso)
            .HasName("ivs_visa_country_territory_cities$city_iso")
            .IsUnique();

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.SerialNum)
            .HasName("serial_num_idx");

        entity.Property(e => e.CityId)
            .HasColumnName("city_id")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CityIso)
            .HasColumnName("city_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CityName)
            .IsRequired()
            .HasColumnName("city_name")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.SerialNum)
            .HasColumnName("serial_num")
            .ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<IvsVisaHelpAddress>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_help_address", "ivsource");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.HasIndex(e => e.IsEnable)
            .HasName("is_enable");

        entity.HasIndex(e => e.OfficeId)
            .HasName("ivs_visa_help_address$office_id")
            .IsUnique();

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.AddressType)
            .HasColumnName("address_type")
            .HasMaxLength(5)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .IsRequired()
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.OfficeAddress)
            .HasColumnName("office_address")
            .IsUnicode(false);

        entity.Property(e => e.OfficeCity)
            .HasColumnName("office_city")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeCountry)
            .HasColumnName("office_country")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeEmail)
            .HasColumnName("office_email")
            .IsUnicode(false);

        entity.Property(e => e.OfficeFax)
            .HasColumnName("office_fax")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeId)
            .IsRequired()
            .HasColumnName("office_id")
            .HasMaxLength(15)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeName)
            .HasColumnName("office_name")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeNotes)
            .HasColumnName("office_notes")
            .IsUnicode(false);

        entity.Property(e => e.OfficePhone)
            .HasColumnName("office_phone")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficePincode)
            .HasColumnName("office_pincode")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeUrl)
            .HasColumnName("office_url")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.OfficeWebsite)
            .HasColumnName("office_website")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      modelBuilder.Entity<IvsVisaInformation>(entity =>
      {
        entity.HasKey(e => e.SerialNum);

        entity.ToTable("ivs_visa_information", "ivsource");

        entity.HasIndex(e => e.CityId)
            .HasName("city_id_idx");

        entity.HasIndex(e => e.CountryIso)
            .HasName("country_iso_idx");

        entity.Property(e => e.SerialNum).HasColumnName("serial_num");

        entity.Property(e => e.CityId)
            .HasColumnName("city_id")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.Priority)
            .HasColumnName("priority")
            .HasMaxLength(100)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'100')");

        entity.Property(e => e.VisaGeneralInformation)
            .HasColumnName("visa_general_information")
            .IsUnicode(false);

        entity.Property(e => e.VisaInformation)
            .HasColumnName("visa_information")
            .IsUnicode(false);
      });

      modelBuilder.Entity<IvsVisaMenus>(entity =>
      {
        entity.ToTable("IVS_VISA_MENUS");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.Link)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode(false);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        entity.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(40)
            .IsUnicode(false);
      });

      modelBuilder.Entity<IvsVisaPages>(entity =>
      {
        entity.ToTable("IVS_VISA_PAGES");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.Description).IsUnicode(false);
        entity.Property(e => e.IsEnable).IsUnicode(true);

          entity.Property(e => e.Image)
            .HasMaxLength(200)
            .IsUnicode(false);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode(false);

        entity.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(40)
            .IsUnicode(false);
      });

      modelBuilder.Entity<IvsVisaSaarcDetails>(entity =>
      {
        entity.HasKey(e => e.VisaOfficeId);

        entity.ToTable("ivs_visa_saarc_details", "ivsource");

        entity.HasIndex(e => e.SerialNum)
            .HasName("serial_num_idx");

        entity.Property(e => e.VisaOfficeId)
            .HasColumnName("visa_office_id")
            .HasMaxLength(20)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryId)
            .HasColumnName("country_id")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CountryIso)
            .IsRequired()
            .HasColumnName("country_iso")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.IsEnable)
            .HasColumnName("is_enable")
            .HasDefaultValueSql("((0))");

        entity.Property(e => e.IsVisaRequired)
            .HasColumnName("is_visa_required")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.ModifiedDate)
            .HasColumnName("modified_date")
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        entity.Property(e => e.SerialNum)
            .HasColumnName("serial_num")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.VisaApplyWhere)
            .HasColumnName("visa_apply_where")
            .HasMaxLength(50)
            .IsUnicode(false);

        entity.Property(e => e.VisaOfficeAddress)
            .HasColumnName("visa_office_address")
            .IsUnicode(false);

        entity.Property(e => e.VisaOfficeCity)
            .HasColumnName("visa_office_city")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaOfficeCountry)
            .HasColumnName("visa_office_country")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaOfficeEmail)
            .HasColumnName("visa_office_email")
            .IsUnicode(false);

        entity.Property(e => e.VisaOfficeFax)
            .HasColumnName("visa_office_fax")
            .HasMaxLength(100)
            .IsUnicode(false);

        entity.Property(e => e.VisaOfficeName)
            .HasColumnName("visa_office_name")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaOfficeNotes)
            .HasColumnName("visa_office_notes")
            .IsUnicode(false);

        entity.Property(e => e.VisaOfficePincode)
            .HasColumnName("visa_office_pincode")
            .HasMaxLength(10)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaOfficeTelephone)
            .HasColumnName("visa_office_telephone")
            .HasMaxLength(100)
            .IsUnicode(false);

        entity.Property(e => e.VisaOfficeUrl)
            .HasColumnName("visa_office_url")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");

        entity.Property(e => e.VisaOfficeWebsite)
            .HasColumnName("visa_office_website")
            .HasMaxLength(200)
            .IsUnicode(false)
            .HasDefaultValueSql("(N'')");
      });

      modelBuilder.Entity<IvsVisaSubPages>(entity =>
      {
        entity.ToTable("IVS_VISA_SUB_PAGES");

        entity.Property(e => e.Id).HasColumnName("ID");

        entity.Property(e => e.CreatedDate).HasColumnType("smalldatetime");

        entity.Property(e => e.Description).IsUnicode(false);

        entity.Property(e => e.Image)
            .HasMaxLength(200)
            .IsUnicode(false);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode(false);
      });

      modelBuilder.Entity<IvsVisaQuickContactInfo>(entity =>
      {
          entity.ToTable("ivs_visa_quick_contact_info", "ivsource");

          entity.HasKey(e => e.ID);

          entity.Property(e => e.CompanyName).HasColumnName("CompanyName");

          entity.Property(e => e.Address).HasColumnName("Address");

          entity.Property(e => e.Email).HasColumnName("Email");

          entity.Property(e => e.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("(N'')");
      });
    }
  }
}
