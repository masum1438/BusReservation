using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

/// <summary>
/// Base controller providing helpers to read the authenticated user's identity from JWT claims.
/// FIX: Replaces all Guid.NewGuid() placeholders that were previously used as userId.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns the current user's Guid from the NameIdentifier claim.
    /// Throws if the claim is missing or malformed — caller should be behind [Authorize].
    /// </summary>
    protected Guid CurrentUserId
    {
        get
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
                throw new UnauthorizedAccessException("User identity claim is missing or invalid");
            return id;
        }
    }

    protected string CurrentUsername
        => User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

    protected bool IsAdmin
        => User.IsInRole("Admin");
}