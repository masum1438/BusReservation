using Domain.Entities;

namespace Domain.Services;

public interface ISeatAvailabilityService
{
    Task<bool> IsSeatAvailableAsync(Guid seatId);
    Task<bool> ValidateSeatBookingAsync(Guid busScheduleId, string seatNumber);
    Task<Seat> BookSeatAsync(Guid seatId, Passenger passenger);
    Task ReleaseSeatAsync(Guid seatId);

    Task<int> GetAvailableSeatsCountAsync(Guid busScheduleId);
    Task<List<string>> GetAvailableSeatsAsync(Guid busScheduleId);
    Task<bool> AreSeatsAvailableAsync(Guid busScheduleId, List<string> seatNumbers);
    Task<Dictionary<string, bool>> CheckSeatsAvailabilityAsync(Guid busScheduleId, List<string> seatNumbers);

    Task<bool> LockSeatForBookingAsync(Guid seatId, Guid userId, int durationMinutes = 10);
    Task<bool> UnlockSeatAsync(Guid seatId, Guid userId);
    Task CleanExpiredLocksAsync();
}