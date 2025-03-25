using BusinessObjects.Dtos.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.Bill
{
    public class BillDto
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public int? TicketId { get; set; }
        public TicketDto? Ticket { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public string? TransactionStatus { get; set; }
    }
} 