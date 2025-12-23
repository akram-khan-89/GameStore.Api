using System;

namespace GameStore.Api.Entities;

// Data Model
public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
