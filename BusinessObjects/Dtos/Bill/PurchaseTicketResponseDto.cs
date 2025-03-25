using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Bill
{
    public class PurchaseTicketResponseDto
    {
        public double TotalPrice { get; set; }
        public string? MovieName { get; set; }
        public DateTime ShowDateTime { get; set; }
        public string Status { get; set; }
    }
}
