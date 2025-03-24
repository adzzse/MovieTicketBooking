using BusinessObjects;
using BusinessObjects.Dtos.Bill;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IBillService : IGenericService<Bill>
    {
        Task<PurchaseTicketResponseDto> PurchaseTickets(int showtimeId, List<int> seatIds, int userId);
        Task<List<Bill>> GetBillsByAccountId(int accountId);
        Task<Boolean> CheckBill(int ticketId);
    }
}
