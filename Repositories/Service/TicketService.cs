using BusinessObjects;
using DataAccessLayers;
using DataAccessLayers.UnitOfWork;
using Microsoft.VisualBasic;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class TicketService(GenericRepository<Ticket> ticketDAO, IUnitOfWork unitOfWork) : GenericService<Ticket>(unitOfWork), ITicketService
    {
        public async Task<int?> CountQuantityPeopleJoinEvent(Movie eventName)
        {
            var totalTickets = eventName.Tickets.Count();
            var currentTicket = await _unitOfWork.TicketRepository.GetRemainingTicketsForEvent(eventName.Id);
            return totalTickets - currentTicket;
        }

        public async Task<List<Ticket>> GetByMovieIdAsync(int eventId)
        {
           var tickets =  await _unitOfWork.TicketRepository.FindAsync(a => a.MovieId == eventId);
            return tickets.ToList();
        }

        public async Task UpdateNewTicket(Ticket ticket)
        {
            await _unitOfWork.TicketRepository.UpdateNew(ticket);
        }

        public async Task<IEnumerable<Ticket>> GetAllIncludeAsync() => await _unitOfWork.TicketRepository.GetAllIncludeAsync();
        public async Task<Ticket?> GetByIdInclude(int id) => await _unitOfWork.TicketRepository.GetByIdInclude(id);
        public async Task<List<Ticket>> GetByMovieIdInclude(int movieId) => await _unitOfWork.TicketRepository.GetByMovieIdInclude(movieId);
    }
}
