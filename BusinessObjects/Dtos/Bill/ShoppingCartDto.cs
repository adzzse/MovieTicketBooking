using BusinessObjects.Dtos.Ticket;
using System;
using System.Collections.Generic;

namespace BusinessObjects.Dtos.Bill
{
    public class ShoppingCartDto
    {
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
        public int TotalPrice { get; set; }
        public int? AccountId { get; set; }
        public string? PaymentUrl { get; set; }
    }
} 