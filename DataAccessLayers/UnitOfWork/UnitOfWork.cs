﻿using BusinessObjects;

namespace DataAccessLayers.UnitOfWork
{
    public class UnitOfWork(MovieprojectContext context) : IUnitOfWork
    {
        private readonly MovieprojectContext _projectContext = context;
        private CategoryRepository categoryRepository;
        private AccountRepository accountRepository;
        private MovieRepository eventRepository;
        private RoleRepository roleRepository;
        private BillRepository billRepository;

        public AccountRepository AccountRepository
        {
            get
            {
                accountRepository ??= new AccountRepository(_projectContext);
                return accountRepository;
            }
        }

        public CategoryRepository CategoryRepository
        {
            get
            {
                categoryRepository ??= new CategoryRepository(_projectContext);
                return categoryRepository;
            }
        }

        public MovieRepository EventRepository
        {
            get
            {
                eventRepository ??= new MovieRepository(_projectContext);
                return eventRepository;
            }
        }

        public RoleRepository RoleRepository
        {
            get
            {
                roleRepository ??= new RoleRepository(_projectContext);
                return roleRepository;
            }
        }

        public BillRepository BillRepository
        {
            get
            {
                billRepository ??= new BillRepository(_projectContext);
                return billRepository;
            }
        }

        private TicketRepository ticketRepository;
        public TicketRepository TicketRepository
        {
            get
            {
                ticketRepository ??= new TicketRepository(_projectContext);
                return ticketRepository;
            }
        }

        private TransactionRepository transactionRepository;
        public TransactionRepository TransactionRepository
        {
            get
            {
                transactionRepository ??= new TransactionRepository(_projectContext);
                return transactionRepository;
            }
        }

        private TransactionHistoryRepository transactionHistoryRepository;
        public TransactionHistoryRepository TransactionHistoryRepository
        {
            get
            {
                transactionHistoryRepository ??= new TransactionHistoryRepository(_projectContext);
                return transactionHistoryRepository;
            }
        }

        private TransactionTypeRepository transactionTypeRepository;
        public TransactionTypeRepository TransactionTypeRepository
        {
            get
            {
                transactionTypeRepository ??= new TransactionTypeRepository(_projectContext);
                return transactionTypeRepository;
            }
        }

        private SeatRepository seatRepository;
        public SeatRepository SeatRepository
        {
            get
            {
                seatRepository ??= new SeatRepository(_projectContext);
                return seatRepository;
            }
        }

        private CinemaRoomRepository cinemaRoomRepository;
        public CinemaRoomRepository CinemaRoomRepository
        {
            get
            {
                cinemaRoomRepository ??= new CinemaRoomRepository(_projectContext);
                return cinemaRoomRepository;
            }
        }

        private ShowTimeRepository showTimeRepository;
        public ShowTimeRepository ShowTimeRepository
        {
            get
            {
                showTimeRepository ??= new ShowTimeRepository(_projectContext);
                return showTimeRepository;
            }
        }

        public GenericRepository<E> GenericRepository<E>() where E : class
        {
            return new GenericRepository<E>(_projectContext);
        }

        public async Task SaveChangesAsync()
        {
            await _projectContext.SaveChangesAsync();
        }
    }
}
