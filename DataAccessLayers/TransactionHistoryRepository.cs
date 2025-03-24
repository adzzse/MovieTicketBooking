using BusinessObjects;
using BusinessObjects.Dtos.TransactionHistory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers
{
    public class TransactionHistoryRepository : GenericRepository<TransactionHistory>
    {
        public TransactionHistoryRepository(MovieprojectContext context) : base(context)
        {
        }

        public async Task<List<TransactionHistoryDto>> GetAllTransactionHistoryByAccountId(int accountId)
        {
            var transactionHistories = await _context.TransactionHistories
                .Include(th => th.Transaction)
                .ThenInclude(t => t.Bill)
                .ThenInclude(b => b.Ticket)
                .ThenInclude(t => t.Movie)
                .Include(th => th.Transaction)
                .ThenInclude(t => t.Bill)
                .ThenInclude(b => b.Ticket)
                .ThenInclude(t => t.Showtime)
                .ThenInclude(st => st.CinemaRoom)
                .Include(th => th.Transaction)
                .ThenInclude(t => t.Bill)
                .ThenInclude(b => b.Ticket)
                .ThenInclude(t => t.Seat)
                .Include(th => th.Transaction)
                .ThenInclude(t => t.Type)
                .Where(th => th.AccountId == accountId)
                .OrderByDescending(t => t.Time)
                .Select(t => new TransactionHistoryDto
                {
                    MovieName = t.Transaction!.Bill!.Ticket!.Movie!.Name,
                    ShowDateTime = t.Transaction!.Bill!.Ticket!.Showtime!.ShowDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                    Room = t.Transaction!.Bill!.Ticket!.Showtime!.CinemaRoom!.RoomName,
                    SeatName = t.Transaction!.Bill!.Ticket!.Seat!.SeatNumber,
                    TicketQuantity = t.Transaction!.Bill!.Quantity,
                    TotalPrice = t.Transaction!.Bill!.TotalPrice,
                    Time = t.Time!.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    Status = t.Status ?? "Unknown",
                    TransactionType = t.Transaction!.Type!.Name
                })
                .ToListAsync();

            return transactionHistories;
        }

        public async Task<IEnumerable<TransactionHistory>> GetAllIncludeAsync()
        {
            return await _context.TransactionHistories
                .Include(th => th.Transaction)
                .Include(th => th.Account)
                .ToListAsync();
        }
    }
}
