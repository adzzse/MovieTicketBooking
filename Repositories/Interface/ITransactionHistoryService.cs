using BusinessObjects;
using BusinessObjects.Dtos.Transaction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ITransactionHistoryService : IGenericService<TransactionHistory>
    {
        Task<List<TransactionHistoryDto>> GetUserTransactionHistory(int accountId);
    }
}
