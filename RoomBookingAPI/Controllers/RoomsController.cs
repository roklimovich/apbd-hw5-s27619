using Microsoft.AspNetCore.Mvc;
using RoomBookingAPI.Models;

namespace RoomBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    // GET /api/rooms
    // GET /api/rooms?minCapacity=20&hasProjector=true&activeOnly=true
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] int? minCapacity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        var rooms = DataStore.Rooms.AsEnumerable();

        if (minCapacity.HasValue)
            rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);

        if (hasProjector.HasValue)
            rooms = rooms.Where(r => r.HasProjector == hasProjector.Value);

        if (activeOnly.HasValue && activeOnly.Value)
            rooms = rooms.Where(r => r.IsActive);

        return Ok(rooms.ToList());
    }

    // GET /api/rooms/{id}
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room is null)
            return NotFound(new { message = $"Room with id {id} not found." });

        return Ok(room);
    }

    // GET /api/rooms/building/{buildingCode}
    [HttpGet("building/{buildingCode}")]
    public IActionResult GetByBuilding(string buildingCode)
    {
        var rooms = DataStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(rooms);
    }

    // POST /api/rooms
    [HttpPost]
    public IActionResult Create([FromBody] Room room)
    {
        room.Id = DataStore.NextRoomId();
        DataStore.Rooms.Add(room);

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    // PUT /api/rooms/{id}
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Room updated)
    {
        var existing = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (existing is null)
            return NotFound(new { message = $"Room with id {id} not found." });

        existing.Name         = updated.Name;
        existing.BuildingCode = updated.BuildingCode;
        existing.Floor        = updated.Floor;
        existing.Capacity     = updated.Capacity;
        existing.HasProjector = updated.HasProjector;
        existing.IsActive     = updated.IsActive;

        return Ok(existing);
    }

    // DELETE /api/rooms/{id}
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room is null)
            return NotFound(new { message = $"Room with id {id} not found." });

        // Block deletion when any related reservation exists
        bool hasReservations = DataStore.Reservations.Any(res => res.RoomId == id);
        if (hasReservations)
            return Conflict(new { message = "Cannot delete a room that has existing reservations." });

        DataStore.Rooms.Remove(room);
        return NoContent();
    }
}
