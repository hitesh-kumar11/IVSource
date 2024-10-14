using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaCategoriesOptionsObj
    {
        [Required]
        public List<IvsVisaCategories> Categories { get; set; }
        [Required]
        public IvsVisaCategoriesOptions categoriesOptions { get; set; }        
    }

}
