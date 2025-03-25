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
        
        // New methods for the updated flow
        Task<ShoppingCartDto> AddTicketToCart(int ticketId, int? accountId);
        Task<ShoppingCartDto> GetShoppingCart(List<int> ticketIds, int? accountId);
        Task<PurchaseTicketResponseDto> ConfirmPurchase(ConfirmPurchaseDto confirmDto);
    }
}
