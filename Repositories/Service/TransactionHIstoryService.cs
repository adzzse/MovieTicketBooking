using BusinessObjects;
using BusinessObjects.Dtos.TransactionHistory;
using DataAccessLayers;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Service
{
    public class TransactionHIstoryService(GenericRepository<TransactionHistory> transactionHistoryDAO, IUnitOfWork unitOfWork) : GenericService<TransactionHistory>(unitOfWork), ITransactionHistoryService
    {
        private readonly GenericRepository<TransactionHistory> _transactionHistoryDAO = transactionHistoryDAO;

        public async Task<List<TransactionHistory>> GetTransactionHistoryByAccountId(int accountId)
        {
            var transactionHistory = await _unitOfWork.TransactionHistoryRepository.FindAsync(a => a.Transaction!.Bill!.AccountId == accountId);
            return transactionHistory.ToList();
        }

        public async Task <List<TransactionHistoryDto>> GetAllTransactionHistoryByAccountId(int accountId)
        {
            return await _unitOfWork.TransactionHistoryRepository.GetAllTransactionHistoryByAccountId(accountId);
        }

        public async Task<List<BusinessObjects.Dtos.Transaction.TransactionHistoryDto>> GetUserTransactionHistory(int accountId)
        {
            // Validate account exists
            var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                throw new Exception("User account not found");
            }

            // Get all bills for this account
            var bills = await _unitOfWork.BillRepository.FindAsync(b => b.AccountId == accountId);
            if (bills == null || !bills.Any())
            {
                return new List<BusinessObjects.Dtos.Transaction.TransactionHistoryDto>();
            }

            // Get all transaction IDs from the bills
            var billIds = bills.Select(b => b.Id).ToList();
            var transactions = await _unitOfWork.TransactionRepository.FindAsync(t => billIds.Contains(t.BillId ?? 0));
            
            var transactionDtos = new List<BusinessObjects.Dtos.Transaction.TransactionHistoryDto>();
            
            foreach (var transaction in transactions)
            {
                // Find the associated bill
                var bill = bills.FirstOrDefault(b => b.Id == transaction.BillId);
                if (bill == null || bill.TicketId == null) continue;
                
                // Get ticket details
                var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(bill.TicketId.Value);
                if (ticket == null) continue;
                
                // Get seat, showtime and movie details
                var seat = await _unitOfWork.SeatRepository.GetByIdAsync(ticket.SeatId);
                var showtime = await _unitOfWork.ShowTimeRepository.GetByIdAsync(ticket.ShowtimeId);
                var movie = await _unitOfWork.MovieRepository.GetByIdAsync(ticket.MovieId);
                
                // Get transaction histories if any
                var histories = await _unitOfWork.TransactionHistoryRepository.FindAsync(th => th.TransactionId == transaction.Id);
                var latestHistory = histories.OrderByDescending(h => h.Time).FirstOrDefault();
                
                transactionDtos.Add(new BusinessObjects.Dtos.Transaction.TransactionHistoryDto
                {
                    Id = bill.Id,
                    TransactionId = transaction.Id,
                    MovieName = movie?.Name ?? "Unknown movie",
                    SeatNumber = seat?.SeatNumber ?? "Unknown seat",
                    ShowDateTime = showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = bill.TotalPrice,
                    TransactionDate = latestHistory?.Time ?? DateTime.Now,
                    Status = transaction.Status ?? "Unknown",
                    PaymentReference = latestHistory?.Status ?? transaction.Status ?? "Unknown"
                });
            }
            
            // Order by most recent transactions first
            return transactionDtos.OrderByDescending(t => t.TransactionDate).ToList();
        }
    }
}
