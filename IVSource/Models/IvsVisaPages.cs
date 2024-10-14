using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IVSource.Models
{
    public partial class IvsVisaPages
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        [Display(Name = "Enable")]
        public bool IsEnable { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class IvsVisaPagesLinks
    {
        public List<IvsVisaPages> Linkr { get; set; }
    }

    public class IvsVisaPagesOurContent
    {
        public List<IvsVisaPages> Pager { get; set; }
    }
}
