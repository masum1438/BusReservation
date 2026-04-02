using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;

namespace Infrastructure.Services;

public class SeatAvailabilityService : ISeatAvailabilityService
{
    private readonly IUnitOfWork _unitOfWork;

    public SeatAvailabilityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsSeatAvailableAsync(Guid seatId)
    {
        var seat = await _unitOfWork.Repository<Seat>().GetByIdAsync(seatId);
        if (seat == null || seat.Status != SeatStatus.Available)
            return false;

        var activeLocks = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
            l.SeatId == seatId && l.IsActive && l.ExpiresAt > DateTime.UtcNow);

        return !activeLocks.Any();
    }

    public async Task<bool> ValidateSeatBookingAsync(Guid busScheduleId, string seatNumber)
    {
        var seats = await _unitOfWork.Repository<Seat>().FindAsync(s =>
            s.BusScheduleId == busScheduleId && s.SeatNumber == seatNumber);
        var seat = seats.FirstOrDefault();
        return seat != null && await IsSeatAvailableAsync(seat.Id);
    }

    public async Task<Seat> BookSeatAsync(Guid seatId, Passenger passenger)
    {
        var seatRepo = _unitOfWork.Repository<Seat>();
        var seat = await seatRepo.GetByIdAsync(seatId)
            ?? throw new KeyNotFoundException("Seat not found");

        if (seat.Status != SeatStatus.Available)
            throw new InvalidOperationException("Seat is not available");

        var activeLocks = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
            l.SeatId == seatId && l.IsActive && l.ExpiresAt > DateTime.UtcNow);

        if (activeLocks.Any())
            throw new InvalidOperationException("Seat is locked by another user");

        seat.Status = SeatStatus.Booked;
        seat.UpdatedAt = DateTime.UtcNow;
        await seatRepo.UpdateAsync(seat);

        return seat;
    }

    public async Task ReleaseSeatAsync(Guid seatId)
    {
        var seatRepo = _unitOfWork.Repository<Seat>();
        var seat = await seatRepo.GetByIdAsync(seatId);
        if (seat != null && seat.Status == SeatStatus.Booked)
        {
            seat.Status = SeatStatus.Available;
            seat.UpdatedAt = DateTime.UtcNow;
            await seatRepo.UpdateAsync(seat);
        }
    }

    public async Task<int> GetAvailableSeatsCountAsync(Guid busScheduleId)
    {
        var seats = await _unitOfWork.Repository<Seat>()
            .FindAsync(s => s.BusScheduleId == busScheduleId);
        int count = 0;
        foreach (var seat in seats.Where(s => s.Status == SeatStatus.Available))
        {
            var locks = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
                l.SeatId == seat.Id && l.IsActive && l.ExpiresAt > DateTime.UtcNow);
            if (!locks.Any()) count++;
        }
        return count;
    }

    public async Task<List<string>> GetAvailableSeatsAsync(Guid busScheduleId)
    {
        var seats = await _unitOfWork.Repository<Seat>()
            .FindAsync(s => s.BusScheduleId == busScheduleId);
        var result = new List<string>();
        foreach (var seat in seats.Where(s => s.Status == SeatStatus.Available))
        {
            var locks = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
                l.SeatId == seat.Id && l.IsActive && l.ExpiresAt > DateTime.UtcNow);
            if (!locks.Any()) result.Add(seat.SeatNumber);
        }
        return result;
    }

    public async Task<bool> AreSeatsAvailableAsync(Guid busScheduleId, List<string> seatNumbers)
    {
        foreach (var seatNumber in seatNumbers)
        {
            if (!await ValidateSeatBookingAsync(busScheduleId, seatNumber))
                return false;
        }
        return true;
    }

    public async Task<Dictionary<string, bool>> CheckSeatsAvailabilityAsync(Guid busScheduleId, List<string> seatNumbers)
    {
        var result = new Dictionary<string, bool>();
        foreach (var seatNumber in seatNumbers)
            result[seatNumber] = await ValidateSeatBookingAsync(busScheduleId, seatNumber);
        return result;
    }

    public async Task<bool> LockSeatForBookingAsync(Guid seatId, Guid userId, int durationMinutes = 10)
    {
        var existing = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
            l.SeatId == seatId && l.IsActive && l.ExpiresAt > DateTime.UtcNow);
        if (existing.Any()) return false;

        await _unitOfWork.Repository<SeatLock>().AddAsync(new SeatLock
        {
            Id = Guid.NewGuid(),
            SeatId = seatId,
            UserId = userId,
            LockedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(durationMinutes),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlockSeatAsync(Guid seatId, Guid userId)
    {
        var locks = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
            l.SeatId == seatId && l.UserId == userId && l.IsActive);
        var seatLock = locks.FirstOrDefault();
        if (seatLock == null) return false;

        seatLock.IsActive = false;
        seatLock.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Repository<SeatLock>().UpdateAsync(seatLock);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task CleanExpiredLocksAsync()
    {
        var expired = await _unitOfWork.Repository<SeatLock>()
            .FindAsync(l => l.IsActive && l.ExpiresAt <= DateTime.UtcNow);
        foreach (var lockItem in expired)
        {
            lockItem.IsActive = false;
            lockItem.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<SeatLock>().UpdateAsync(lockItem);
        }
        await _unitOfWork.SaveChangesAsync();
    }
}