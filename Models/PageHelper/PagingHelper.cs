using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PageHelper
{
    public static class PagingHelper<T> where T : class
    {
        public static PageResults<T>? Paging (List<T> list, int? page, int? pageSize)
        {
            try
            {
                if (page == null && pageSize == null)
                {
                    pageSize = list.Count;
                    page = 1;
                }else if (page < 1 || pageSize < 1)
                {
                    return null;
                }
                var skipAmount = pageSize * (page - 1);
                var totalNumberOfRecords = list.Count;
                var results = list.Skip((int)skipAmount).Take(pageSize.Value).ToList();
                var mod = totalNumberOfRecords % pageSize;
                var totalPages = totalNumberOfRecords / pageSize + (mod ==0 ? 0 : 1);
                return new PageResults<T>
                {
                    PageNumber = page.Value,
                    PageSize = pageSize.Value,
                    TotalPages = (int)totalPages,
                    TotalRecords = totalNumberOfRecords,
                    Results = results
                };  
            } catch(Exception ex)
            {
                return null;
            }
        }
    }
}
