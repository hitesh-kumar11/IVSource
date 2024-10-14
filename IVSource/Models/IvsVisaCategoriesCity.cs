using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    //---------------------------------------------------------Added New 22-03-23
    public class IvsVisaCategoriesCity
    {

        [Display(Name = "Select City")]
        public string city { get; set; }

        [Display(Name = "Select City_Code")]
        public string city_code { get; set; }

        public int is_enable { get; set; }


    }
}

