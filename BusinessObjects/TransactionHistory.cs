using System;
using System.Collections.Generic;

namespace BusinessObjects;

public class TransactionHistory
{
    public int Id { get; set; }
    public int? TransactionId { get; set; }
    public int Price { get; set; }
    public int? AccountId { get; set; }
    public DateTime? Time { get; set; }
    public string? Status { get; set; }

    public virtual Transaction? Transaction { get; set; }
    public virtual Account? Account { get; set; }
}
