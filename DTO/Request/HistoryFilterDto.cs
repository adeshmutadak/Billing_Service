using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class HistoryFilterDto
    {
        public int Year { get; set; }
        public int? CustomerId { get; set; }
        public int? Month { get; set; }
        public bool? IsPaid { get; set; }
        public bool IncludeDetail { get; set; }
        public bool IncludeEmptyDays { get; set; }
    }
}
