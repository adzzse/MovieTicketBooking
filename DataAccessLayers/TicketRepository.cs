using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers
{
    public class TicketRepository(MovieprojectContext context) : GenericRepository<Ticket>(context)
    {
        public async Task<int> GetRemainingTicketsForEvent(int eventId)
        {
            var totalTicketQuantity = await _context.Tickets
                .Where(t => t.MovieId == eventId)
                .CountAsync();

            var soldTicketQuantity = await _context.Bills
                .Where(b => b.Ticket!.MovieId == eventId)
                .CountAsync();

            var remainingTickets = totalTicketQuantity - soldTicketQuantity;
            return remainingTickets;
        }

        public async Task UpdateNew(Ticket ticket)
        {
            if (_context.Entry(ticket).State == EntityState.Detached)
            {
                _context.Tickets.Attach(ticket);
            }
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Ticket?> GetByIdInclude(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .FirstOrDefaultAsync(t => t.Id == id);

            return ticket;
        }

        public async Task<List<Ticket>> GetByMovieIdInclude(int movieId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Seat)
                .Include(t => t.Showtime) 
                .Where(t => t.MovieId == movieId) 
                .ToListAsync();  

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetAllIncludeAsync()
        {
            return await _context.Tickets
                .Include(t => t.Movie)
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .ToListAsync();
        }
    }
}
