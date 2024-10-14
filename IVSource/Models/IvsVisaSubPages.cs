using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IVSource.Models
{
    public partial class IvsVisaSubPages
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        public string Country_ISO { get; set; }     //----------As could not add a NULL value in DB ---#1
        public string Image { get; set; }
        public bool IsEnable { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class IvsVisaSubPagess
    {
        public IvsVisaSubPages IvaVisaSubPagess { get; set; }
    }
}
