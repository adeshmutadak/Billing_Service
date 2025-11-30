using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class MilkEntryResponseDto
    {
        public int EntryId { get; set; }
        public int UserId {get ;set;}
        public int CustomerId { get; set; }
        public DateOnly Date { get; set; }
        public decimal? CowLitre { get; set; }
        public decimal? BuffaloLitre { get; set; }
        public decimal? CowRate { get; set; }
        public decimal? BuffaloRate { get; set; }
        public decimal? TotalAmount { get; set; }
    }

}
