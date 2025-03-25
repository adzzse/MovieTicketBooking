using BusinessObjects;
using BusinessObjects.Dtos.Bill;
using BusinessObjects.Dtos.Ticket;
using System;

namespace BusinessObjects.Mappers
{
    public static class BillMapper
    {
        public static BillDto MapToBillDto(Bill bill)
        {
            return new BillDto
            {
                Id = bill.Id,
                AccountId = bill.AccountId,
                AccountName = bill.Account?.Name,
                TicketId = bill.TicketId,
                Ticket = bill.Ticket == null ? null : new TicketDto
                {
                    Id = bill.Ticket.Id,
                    SeatId = bill.Ticket.SeatId,
                    SeatName = bill.Ticket.Seat?.SeatNumber ?? string.Empty,
                    MovieName = bill.Ticket.Movie?.Name,
                    ShowDateTime = bill.Ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = bill.Ticket.Price,
                    Status = bill.Ticket.Status
                },
                Quantity = bill.Quantity,
                TotalPrice = bill.TotalPrice,
                TransactionStatus = bill.Transaction?.Status
            };
        }
    }
} 