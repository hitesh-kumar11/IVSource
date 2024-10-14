using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class IvsVisaCategoriesFormsObj
    {
        [Required]
        public List<IvsVisaCategories> Categories { get; set; }

        [Required]
        public IvsVisaCategoriesForms categoriesForms { get; set; }
       
    }
}
