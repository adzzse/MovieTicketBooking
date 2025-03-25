using BusinessObjects;
using BusinessObjects.Dtos.Bill;
using BusinessObjects.Dtos.Ticket;
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
        private readonly ITicketService _ticketService;

        public BillService(IUnitOfWork unitOfWork, IAccountService accountRepository, ITicketService ticketService) 
            : base(unitOfWork)
        {
            _accountService = accountRepository;
            _ticketService = ticketService;
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

            var movie = await _unitOfWork.MovieRepository.GetByIdAsync(showtime.MovieId);
            if (movie == null)
            {
                throw new Exception($"Movie not found for showtime {showtimeId}");
            }

            var seats = await _unitOfWork.SeatRepository.FindAsync(s => seatIds.Contains(s.Id));
            if (seats == null || !seats.Any())
            {
                throw new Exception("Seats not found");
            }

            // Calculate total price before creating any records
            var totalPrice = 0;
            var defaultPrice = 120000; // Consider moving this to configuration or calculate based on seat type/movie

            // Begin creating all necessary records
            var bills = new List<Bill>();
            var tickets = new List<Ticket>();
            var transactions = new List<Transaction>();

            try
            {
                // Create tickets first
                foreach (var seat in seats)
                {
                    // Check if ticket exists
                    var existingTicket = await _unitOfWork.TicketRepository.FindOneAsync(t =>
                        t.ShowtimeId == showtimeId && t.SeatId == seat.Id);

                    if (existingTicket == null)
                    {
                        // Create new ticket
                        var newTicket = new Ticket
                        {
                            MovieId = showtime.MovieId,
                            SeatId = seat.Id,
                            ShowtimeId = showtimeId,
                            Price = defaultPrice,
                            Status = 1 // Available ticket
                        };

                        // Add to database
                        await _unitOfWork.TicketRepository.AddAsync(newTicket);
                        
                        // Save immediately to get the ticket ID
                        await _unitOfWork.SaveChangesAsync();
                        
                        // Use the newly created ticket
                        existingTicket = newTicket;
                    }
                    else if (existingTicket.Status == 0)
                    {
                        // Ticket exists but is already sold
                        throw new Exception($"Seat {seat.SeatNumber} is already booked for this showtime.");
                    }

                    // Mark the ticket as sold
                    existingTicket.Status = 0; // Sold ticket
                    await _unitOfWork.TicketRepository.UpdateAsync(existingTicket);

                    // Create bill for this ticket
                    var bill = new Bill
                    {
                        AccountId = account.Id,
                        TicketId = existingTicket.Id,
                        Quantity = 1,
                        TotalPrice = existingTicket.Price
                    };
                    
                    bills.Add(bill);
                    totalPrice += existingTicket.Price;
                    tickets.Add(existingTicket);
                }

                // Add all bills to database
                await _unitOfWork.BillRepository.AddRangeAsync(bills);
                await _unitOfWork.SaveChangesAsync();

                // Create transactions for each bill
                foreach (var bill in bills)
                {
                    transactions.Add(new Transaction
                    {
                        BillId = bill.Id,
                        TypeId = 1,
                        Status = "Success"
                    });
                }

                // Add transactions
                await _unitOfWork.TransactionRepository.AddRangeAsync(transactions);

                // Update showtime available seats
                showtime.AvailableSeats -= seatIds.Count;
                await _unitOfWork.ShowTimeRepository.UpdateAsync(showtime);

                // Save all changes
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error during ticket purchase: {ex.Message}");
                throw new Exception($"Failed to complete purchase: {ex.Message}");
            }

            return new PurchaseTicketResponseDto
            {
                Status = "Success",
                TotalPrice = totalPrice,
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

        public async Task<ShoppingCartDto> AddTicketToCart(int ticketId, int? accountId)
        {
            var ticket = await _ticketService.GetByIdInclude(ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found");
            }

            if (ticket.Status != 1) // 1 means available
            {
                throw new Exception("Ticket is not available");
            }

            var cart = new ShoppingCartDto
            {
                AccountId = accountId,
                Tickets = new List<TicketDto>
                {
                    new TicketDto
                    {
                        Id = ticket.Id,
                        SeatId = ticket.SeatId,
                        SeatName = ticket.Seat?.SeatNumber ?? "Unknown",
                        MovieName = ticket.Movie?.Name,
                        ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                        Price = ticket.Price,
                        Status = ticket.Status
                    }
                },
                TotalPrice = ticket.Price
            };

            return cart;
        }

        public async Task<ShoppingCartDto> GetShoppingCart(List<int> ticketIds, int? accountId)
        {
            var tickets = new List<TicketDto>();
            var totalPrice = 0;

            foreach (var ticketId in ticketIds)
            {
                var ticket = await _ticketService.GetByIdInclude(ticketId);
                if (ticket == null)
                {
                    throw new Exception($"Ticket {ticketId} not found");
                }

                if (ticket.Status != 1) // 1 means available
                {
                    throw new Exception($"Ticket {ticketId} is not available");
                }

                tickets.Add(new TicketDto
                {
                    Id = ticket.Id,
                    SeatId = ticket.SeatId,
                    SeatName = ticket.Seat?.SeatNumber ?? "Unknown",
                    MovieName = ticket.Movie?.Name,
                    ShowDateTime = ticket.Showtime?.ShowDateTime ?? DateTime.MinValue,
                    Price = ticket.Price,
                    Status = ticket.Status
                });

                totalPrice += ticket.Price;
            }

            return new ShoppingCartDto
            {
                AccountId = accountId,
                Tickets = tickets,
                TotalPrice = totalPrice
            };
        }

        public async Task<PurchaseTicketResponseDto> ConfirmPurchase(ConfirmPurchaseDto confirmDto)
        {
            var account = confirmDto.AccountId.HasValue 
                ? await _unitOfWork.AccountRepository.GetByIdAsync(confirmDto.AccountId.Value)
                : null;

            if (account == null)
            {
                throw new Exception("User account not found");
            }

            // Determine payment status from the PaymentReference
            PaymentStatus paymentStatus;
            if (string.IsNullOrEmpty(confirmDto.PaymentReference))
            {
                throw new Exception("Payment reference is required");
            }
            
            // Check if payment reference indicates success
            if (confirmDto.PaymentReference.Contains("success"))
            {
                paymentStatus = PaymentStatus.Success;
            }
            else if (confirmDto.PaymentReference.Contains("cancel"))
            {
                paymentStatus = PaymentStatus.Cancel;
            }
            else
            {
                paymentStatus = PaymentStatus.Error;
            }

            var tickets = new List<Ticket>();
            var totalPrice = 0;

            // Verify all tickets are available
            foreach (var ticketId in confirmDto.TicketIds)
            {
                var ticket = await _ticketService.GetByIdInclude(ticketId);
                if (ticket == null)
                {
                    throw new Exception($"Ticket {ticketId} not found");
                }

                if (ticket.Status != 1) // 1 means available
                {
                    throw new Exception($"Ticket {ticketId} is not available");
                }

                tickets.Add(ticket);
                totalPrice += ticket.Price;
            }

            try
            {
                var bills = new List<Bill>();
                var transactions = new List<Transaction>();

                // Create bills and update tickets
                foreach (var ticket in tickets)
                {
                    // Update ticket status based on payment status
                    ticket.Status = (byte)paymentStatus;
                    await _unitOfWork.TicketRepository.UpdateAsync(ticket);

                    // Create bill
                    var bill = new Bill
                    {
                        AccountId = account.Id,
                        TicketId = ticket.Id,
                        Quantity = 1,
                        TotalPrice = ticket.Price
                    };
                    
                    bills.Add(bill);
                }

                // Add all bills to database
                await _unitOfWork.BillRepository.AddRangeAsync(bills);
                await _unitOfWork.SaveChangesAsync();

                // Create transactions
                foreach (var bill in bills)
                {
                    transactions.Add(new Transaction
                    {
                        BillId = bill.Id,
                        TypeId = 1,
                        Status = paymentStatus == PaymentStatus.Success ? "Success" : "Failed"
                    });
                }

                // Add transactions
                await _unitOfWork.TransactionRepository.AddRangeAsync(transactions);

                // Save all changes
                await _unitOfWork.SaveChangesAsync();

                return new PurchaseTicketResponseDto
                {
                    Status = paymentStatus == PaymentStatus.Success ? "Success" : "Failed",
                    TotalPrice = totalPrice
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error during ticket purchase: {ex.Message}");
                throw new Exception($"Failed to complete purchase: {ex.Message}");
            }
        }
    }
}
