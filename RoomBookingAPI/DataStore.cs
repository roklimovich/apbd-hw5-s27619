using RoomBookingAPI.Models;

namespace RoomBookingAPI;

public static class DataStore
{
    public static List<Room> Rooms { get; } = new()
    {
        new Room { Id = 1, Name = "Lab 101", BuildingCode = "A", Floor = 1, Capacity = 20, HasProjector = true,  IsActive = true  },
        new Room { Id = 2, Name = "Lab 204", BuildingCode = "B", Floor = 2, Capacity = 24, HasProjector = true,  IsActive = true  },
        new Room { Id = 3, Name = "Room 305", BuildingCode = "B", Floor = 3, Capacity = 15, HasProjector = false, IsActive = true  },
        new Room { Id = 4, Name = "Conference A", BuildingCode = "A", Floor = 0, Capacity = 30, HasProjector = true,  IsActive = true  },
        new Room { Id = 5, Name = "Storage 001", BuildingCode = "C", Floor = 1, Capacity = 5,  HasProjector = false, IsActive = false },
    };

    public static List<Reservation> Reservations { get; } = new()
    {
        new Reservation { Id = 1, RoomId = 1, OrganizerName = "Jan Nowak",      Topic = "Intro to C#",              Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(8,  0), EndTime = new TimeOnly(10, 0), Status = "confirmed" },
        new Reservation { Id = 2, RoomId = 1, OrganizerName = "Maria Wiśniewska",Topic = "REST API Design",          Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(13, 0), Status = "planned"   },
        new Reservation { Id = 3, RoomId = 2, OrganizerName = "Anna Kowalska",  Topic = "HTTP and REST Workshop",   Date = new DateOnly(2026, 5, 10), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12,30), Status = "confirmed" },
        new Reservation { Id = 4, RoomId = 4, OrganizerName = "Piotr Zając",    Topic = "Agile Planning",           Date = new DateOnly(2026, 5, 12), StartTime = new TimeOnly(9,  0), EndTime = new TimeOnly(11, 0), Status = "planned"   },
        new Reservation { Id = 5, RoomId = 2, OrganizerName = "Ewa Dąbrowska",  Topic = "Docker Basics",            Date = new DateOnly(2026, 5, 14), StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(15, 0), Status = "cancelled" },
        new Reservation { Id = 6, RoomId = 3, OrganizerName = "Tomasz Lewandowski", Topic = "Git & GitHub",         Date = new DateOnly(2026, 5, 15), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0), Status = "confirmed" },
    };

    private static int _nextRoomId = 6;
    private static int _nextReservationId = 7;

    public static int NextRoomId() => _nextRoomId++;
    public static int NextReservationId() => _nextReservationId++;
}
