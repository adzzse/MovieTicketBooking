using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.UnitOfWork
{
    public interface IUnitOfWork
    {
        public AccountRepository AccountRepository { get; }
        public CategoryRepository CategoryRepository { get; }
        public MovieRepository MovieRepository { get; }
        public RoleRepository RoleRepository { get; }
        public TicketRepository TicketRepository { get; }
        public TransactionRepository TransactionRepository { get; }
        public TransactionHistoryRepository TransactionHistoryRepository { get; }
        public SeatRepository SeatRepository { get; }
        public CinemaRoomRepository CinemaRoomRepository { get; }
        public ShowTimeRepository ShowTimeRepository { get; }
        public BillRepository BillRepository { get; }
        public GenericRepository<E> GenericRepository<E>() where E : class;
        Task SaveChangesAsync();
    }
}
