using BusinessObjects;
using BusinessObjects.Dtos.Bill;
using DataAccessLayers;
using DataAccessLayers.UnitOfWork;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Service
{
    public class BillService : GenericService<Bill>, IBillService
    {
        private readonly IAccountService _accountService;

        public BillService(IUnitOfWork unitOfWork, IAccountService accountRepository) : base(unitOfWork)
        {
            _accountService = accountRepository;
        }

        public async Task<PurchaseTicketResponseDto> PurchaseTickets(int showtimeId, List<int> seatIds, int userId)
        {
            var account = await _unitOfWork.AccountRepository.GetByIdAsync(userId);
            if (account == null)
            {
                throw new Exception("User account not found");
            }

            var showtime = await _unitOfWork.ShowTimeRepository.GetByIdAsync(showtimeId);
            if (showtime == null)
            {
                throw new Exception("Showtime not found");
            }

            var seats = await _unitOfWork.SeatRepository.FindAsync(s => seatIds.Contains(s.Id));
            if (seats == null || !seats.Any())
            {
                throw new Exception("Seats not found");
            }

            var bills = new List<Bill>();
            var totalPrice = 0;

            foreach (var seat in seats)
            {
                var ticket = await _unitOfWork.TicketRepository.FindOneAsync(t =>
                    t.ShowtimeId == showtimeId && t.SeatId == seat.Id);

                if (ticket == null)
                {
                    throw new Exception($"Ticket not found for seat {seat.Id}");
                }

                bills.Add(new Bill
                {
                    AccountId = account.Id,
                    TicketId = ticket.Id,
                    Quantity = 1,
                    TotalPrice = ticket.Price
                });

                totalPrice += ticket.Price;
            }

            if (account.Wallet < totalPrice)
            {
                throw new Exception("Insufficient balance");
            }

            account.Wallet -= totalPrice;
            await _unitOfWork.AccountRepository.UpdateAsync(account);
            await _unitOfWork.BillRepository.AddRangeAsync(bills);

            var transactions = new List<Transaction>();
            foreach (var bill in bills)
            {
                transactions.Add(new Transaction
                {
                    BillId = bill.Id,
                    TypeId = 1,
                    Status = "Success"
                });
            }

            await _unitOfWork.TransactionRepository.AddRangeAsync(transactions);
            await _unitOfWork.SaveChangesAsync();

            return new PurchaseTicketResponseDto
            {
                Status = "Success",
                TotalPrice = totalPrice,
                AccountBalance = account.Wallet
            };
        }

        public async Task<List<Bill>> GetBillsByAccountId(int accountId)
        {
            var bills = await _unitOfWork.BillRepository.FindAsync(a => a.AccountId == accountId);
            return bills.ToList();
        }

        public async Task<Boolean> CheckBill(int ticketId)
        {
            var checkBill = await _unitOfWork.BillRepository.FindAsync(a => a.TicketId == ticketId);
            if (checkBill == null || !checkBill.Any())
            {
                return false;
            }
            return true;
        }
    }
}
