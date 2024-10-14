using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class CountryTable
    {
        [Key]
        public int serial_num { get; set; }
        public string country_name { get; set; }
        public string country_iso { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? modified_date { get; set; }
    }

    public class CountryTables : MemberPageBase
    {
        public List<CountryTable> CountryTablesss { get; set; }
    }
}
