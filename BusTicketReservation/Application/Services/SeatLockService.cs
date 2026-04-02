using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public interface ISeatLockService
{
    Task<SeatLockResponseDto> LockSeatsAsync(SeatLockRequestDto request, Guid userId);
    Task<bool> ReleaseSeatsAsync(SeatReleaseRequestDto request, Guid userId);
    Task<bool> IsSeatLockedAsync(Guid seatId);
    Task CleanExpiredLocksAsync();
}

public class SeatLockService : ISeatLockService
{
    private readonly IUnitOfWork _unitOfWork;

    public SeatLockService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SeatLockResponseDto> LockSeatsAsync(SeatLockRequestDto request, Guid userId)
    {
        var seatLockRepo = _unitOfWork.Repository<SeatLock>();
        var seatRepo = _unitOfWork.Repository<Seat>();
        var lockedSeats = new List<LockedSeatDto>();

        foreach (var seatNumber in request.SeatNumbers)
        {
            var seats = await seatRepo.FindAsync(s =>
                s.BusScheduleId == request.BusScheduleId &&
                s.SeatNumber == seatNumber &&
                !s.IsDeleted);

            var seat = seats.FirstOrDefault();
            if (seat == null)
                return new SeatLockResponseDto { IsSuccess = false, Message = $"Seat {seatNumber} not found" };

            if (seat.Status != SeatStatus.Available)
                return new SeatLockResponseDto { IsSuccess = false, Message = $"Seat {seatNumber} is not available" };

            var existingLocks = await seatLockRepo.FindAsync(l =>
                l.SeatId == seat.Id && l.IsActive && l.ExpiresAt > DateTime.UtcNow);

            if (existingLocks.Any())
                return new SeatLockResponseDto { IsSuccess = false, Message = $"Seat {seatNumber} is already locked" };

            var seatLock = new SeatLock
            {
                Id = Guid.NewGuid(),
                SeatId = seat.Id,
                UserId = userId,
                LockedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(request.LockDurationMinutes),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await seatLockRepo.AddAsync(seatLock);
            lockedSeats.Add(new LockedSeatDto
            {
                SeatNumber = seatNumber,
                LockId = seatLock.Id,
                ExpiresAt = seatLock.ExpiresAt
            });
        }

        await _unitOfWork.SaveChangesAsync();

        return new SeatLockResponseDto
        {
            IsSuccess = true,
            Message = "Seats locked successfully",
            LockedSeats = lockedSeats,
            ExpiresAt = lockedSeats.FirstOrDefault()?.ExpiresAt ?? DateTime.UtcNow
        };
    }

    // FIX: userId passed so only the lock owner can release it
    public async Task<bool> ReleaseSeatsAsync(SeatReleaseRequestDto request, Guid userId)
    {
        var seatLockRepo = _unitOfWork.Repository<SeatLock>();

        foreach (var lockId in request.LockIds)
        {
            var seatLock = await seatLockRepo.GetByIdAsync(lockId);
            if (seatLock == null || !seatLock.IsActive) continue;

            // Only allow the owner to release
            if (seatLock.UserId != userId)
                throw new UnauthorizedAccessException($"Cannot release lock {lockId}: not the owner");

            seatLock.IsActive = false;
            seatLock.UpdatedAt = DateTime.UtcNow;
            await seatLockRepo.UpdateAsync(seatLock);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsSeatLockedAsync(Guid seatId)
    {
        var locks = await _unitOfWork.Repository<SeatLock>().FindAsync(l =>
            l.SeatId == seatId && l.IsActive && l.ExpiresAt > DateTime.UtcNow);
        return locks.Any();
    }

    public async Task CleanExpiredLocksAsync()
    {
        var seatLockRepo = _unitOfWork.Repository<SeatLock>();
        var expiredLocks = await seatLockRepo.FindAsync(l => l.IsActive && l.ExpiresAt <= DateTime.UtcNow);

        foreach (var lockItem in expiredLocks)
        {
            lockItem.IsActive = false;
            lockItem.UpdatedAt = DateTime.UtcNow;
            await seatLockRepo.UpdateAsync(lockItem);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}