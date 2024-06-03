using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Views
{
    public class PageResults<T>
    {
        /// <summary>
        /// The page number this page represents.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        ///  The Size of this page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total page of pages available.  
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The total number of records available.
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// The Url to the next page. - if null, there are no more pages.
        /// </summary>
        public List<T> Results { get; set; }

    }
}
