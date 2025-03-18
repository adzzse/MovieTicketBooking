using System;
using System.Collections.Generic;

namespace BusinessObjects;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public string? Image { get; set; }
    public byte? Status { get; set; }
    public string? DirectorName { get; set; }
    public string? Description { get; set; }

    public virtual Category? Category { get; set; }
    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
