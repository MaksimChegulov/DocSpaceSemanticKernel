namespace DocSpaceSemanticExample.Api.Models;

public record AddUsersToRoomRequest
{
    public required IEnumerable<Invitation> Invitations { get; init; }
}

public record Invitation
{
    public required string Email { get; init; }
    public int Access { get; set; } = 2;
}