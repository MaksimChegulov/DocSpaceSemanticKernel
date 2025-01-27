namespace DocSpaceSemanticExample.Api.Models;

public record CreateRoomRequest
{
    public required string Title { get; init; }
    public required RoomType RoomType { get; init; }
}

public record RoomResponse
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public RoomType RoomType { get; init; }
}