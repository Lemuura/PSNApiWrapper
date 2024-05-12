using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSNApiWrapper.Models
{
    public abstract class Pagination
    {
        public int totalItemCount { get; set; }
        /// <summary>
        /// Returns what the next offset should be to view the next results with the current limit.
        /// </summary>
        public int? nextOffset { get; set; }
        public int? previousOffset { get; set; }
    }
}
