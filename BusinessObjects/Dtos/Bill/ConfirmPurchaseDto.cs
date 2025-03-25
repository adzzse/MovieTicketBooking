using System;
using System.Collections.Generic;

namespace BusinessObjects.Dtos.Bill
{
    public enum PaymentStatus
    {
        Success = 0,
        Error = 1,
        Cancel = 1
    }

    public class ConfirmPurchaseDto
    {
        public List<int> TicketIds { get; set; } = new List<int>();
        public int? AccountId { get; set; }
        public string? PaymentReference { get; set; }
        public PaymentStatus Status { get; set; }
    }
} 