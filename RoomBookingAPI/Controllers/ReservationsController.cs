using Microsoft.AspNetCore.Mvc;
using RoomBookingAPI.Models;

namespace RoomBookingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    // GET /api/reservations
    // GET /api/reservations?date=2026-05-10&status=confirmed&roomId=2
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateOnly? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        var reservations = DataStore.Reservations.AsEnumerable();

        if (date.HasValue)
            reservations = reservations.Where(r => r.Date == date.Value);

        if (!string.IsNullOrWhiteSpace(status))
            reservations = reservations.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (roomId.HasValue)
            reservations = reservations.Where(r => r.RoomId == roomId.Value);

        return Ok(reservations.ToList());
    }

    // GET /api/reservations/{id}
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation is null)
            return NotFound(new { message = $"Reservation with id {id} not found." });

        return Ok(reservation);
    }

    // POST /api/reservations
    [HttpPost]
    public IActionResult Create([FromBody] Reservation reservation)
    {
        // Room must exist
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room is null)
            return NotFound(new { message = $"Room with id {reservation.RoomId} not found." });

        // Room must be active
        if (!room.IsActive)
            return BadRequest(new { message = "Cannot create a reservation for an inactive room." });

        // Check for time overlap on the same day for the same room
        if (HasOverlap(reservation))
            return Conflict(new { message = "The reservation overlaps with an existing reservation for this room." });

        reservation.Id = DataStore.NextReservationId();
        DataStore.Reservations.Add(reservation);

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    // PUT /api/reservations/{id}
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Reservation updated)
    {
        var existing = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (existing is null)
            return NotFound(new { message = $"Reservation with id {id} not found." });

        // Room must exist
        var room = DataStore.Rooms.FirstOrDefault(r => r.Id == updated.RoomId);
        if (room is null)
            return NotFound(new { message = $"Room with id {updated.RoomId} not found." });

        // Room must be active
        if (!room.IsActive)
            return BadRequest(new { message = "Cannot assign reservation to an inactive room." });

        // Check overlap excluding the reservation being updated
        if (HasOverlap(updated, excludeId: id))
            return Conflict(new { message = "The reservation overlaps with an existing reservation for this room." });

        existing.RoomId        = updated.RoomId;
        existing.OrganizerName = updated.OrganizerName;
        existing.Topic         = updated.Topic;
        existing.Date          = updated.Date;
        existing.StartTime     = updated.StartTime;
        existing.EndTime       = updated.EndTime;
        existing.Status        = updated.Status;

        return Ok(existing);
    }

    // DELETE /api/reservations/{id}
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var reservation = DataStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation is null)
            return NotFound(new { message = $"Reservation with id {id} not found." });

        DataStore.Reservations.Remove(reservation);
        return NoContent();
    }


    private static bool HasOverlap(Reservation candidate, int excludeId = -1)
    {
        return DataStore.Reservations.Any(r =>
            r.Id != excludeId &&
            r.RoomId == candidate.RoomId &&
            r.Date == candidate.Date &&
            r.StartTime < candidate.EndTime &&
            candidate.StartTime < r.EndTime);
    }
}
