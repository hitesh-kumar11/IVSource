using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace IVSource.Models
{
    public class PaginatedList<IvsVisaSubPages> : List<IvsVisaSubPages>
    {
        public int PageIndex { get; private set; }

        public int TotalPages { get; private set; }


        public PaginatedList(List<IvsVisaSubPages> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<IvsVisaSubPages> Create(IQueryable<IvsVisaSubPages> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<IvsVisaSubPages>(items, count, pageIndex, pageSize);
        }
    }
}
