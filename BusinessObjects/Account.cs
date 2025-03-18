using System;
using System.Collections.Generic;

namespace BusinessObjects;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public int? RoleId { get; set; }
    public byte? Status { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public float Wallet { get; set; }

    public virtual Role? Role { get; set; }
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();
}
