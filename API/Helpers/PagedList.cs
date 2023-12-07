using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PagedList<T>:List<T>
    {
        public PagedList(IEnumerable<T>items,int pageNumber, int pageSize, int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount/(double)pageSize);
            TotalCount = totalCount;
            AddRange(items);
            
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T>Source,int PageNumber,int PageSize) { 
           var items=await Source.Skip((PageNumber-1) * PageSize).Take(PageSize).ToListAsync();
           var count=await Source.CountAsync();
           return new PagedList<T>(items, PageNumber, PageSize, count);
        } 
    }
}
