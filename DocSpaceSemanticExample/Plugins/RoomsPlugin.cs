using DocSpaceSemanticExample.Api;
using DocSpaceSemanticExample.Api.Models;
using Microsoft.SemanticKernel;

namespace DocSpaceSemanticExample.Plugins;

public class RoomsPlugin(DocSpaceClient client)
{
    [KernelFunction("create_room")]
    public async Task<RoomResponse> CreateRoom(
        string name,
        RoomType roomType)
    {
        return await client.CreateRoom(name, roomType);
    }
    
    [KernelFunction("add_users_to_room")]
    public async Task<bool> AddUsersToRoom(
        int roomId,
        List<string> emails)
    {
        return await client.AddUsersToRoom(roomId, emails);
    }
}