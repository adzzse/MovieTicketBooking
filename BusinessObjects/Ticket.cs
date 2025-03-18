using System;
using System.Collections.Generic;

namespace BusinessObjects;

public class Ticket
{
    public int Id { get; set; }
    public int? MovieId { get; set; }
    public int SeatId { get; set; }
    public int ShowtimeId { get; set; }
    public int Price { get; set; }
    public byte? Status { get; set; }
    public int Quantity { get; set; }

    public virtual Movie? Movie { get; set; }
    public virtual Seat? Seat { get; set; }
    public virtual ShowTime? Showtime { get; set; }
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
