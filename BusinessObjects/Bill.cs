using System;
using System.Collections.Generic;

namespace BusinessObjects;

public class Bill
{
    public int Id { get; set; }
    public int? AccountId { get; set; }
    public int? TicketId { get; set; }
    public int Quantity { get; set; }
    public int TotalPrice { get; set; }

    public virtual Account? Account { get; set; }
    public virtual Ticket? Ticket { get; set; }
    public virtual Transaction? Transaction { get; set; }
} 