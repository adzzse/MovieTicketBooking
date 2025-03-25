using System;
using System.Collections.Generic;

namespace BusinessObjects;

public class Transaction
{
    public int Id { get; set; }
    public int? BillId { get; set; }
    public string? Status { get; set; }

    public virtual Bill? Bill { get; set; }
    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
