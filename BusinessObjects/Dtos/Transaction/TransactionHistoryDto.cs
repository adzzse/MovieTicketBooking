using System;
using System.Collections.Generic;

namespace BusinessObjects.Dtos.Transaction
{
    public class TransactionHistoryDto
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string MovieName { get; set; }
        public string SeatNumber { get; set; }
        public DateTime ShowDateTime { get; set; }
        public int Price { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string PaymentReference { get; set; }
    }
} 