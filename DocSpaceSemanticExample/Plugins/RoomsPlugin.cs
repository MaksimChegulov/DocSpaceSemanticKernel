using System.ComponentModel;
using DocSpaceSemanticExample.Api;
using DocSpaceSemanticExample.Api.Models;
using Microsoft.SemanticKernel;

namespace DocSpaceSemanticExample.Plugins;

public class RoomsPlugin(DocSpaceClient client)
{
    [KernelFunction("create_room")]
    [Description("creates a new room, returns room information")]
    public async Task<RoomResponse> CreateRoom(
        string name,
        [Description("1-FillingFormsRoom,2-EditingRoom,5-CustomRoom,6-PublicRoom,8-VirtualDataRoom")] RoomType roomType)
    {
        return await client.CreateRoomAsync(name, roomType);
    }
    
    [KernelFunction("add_users_to_room")]
    [Description("add/invites users to a room by email; returns true if successful")]
    public async Task<bool> AddUsersToRoom(
        int roomId,
        [Description("[\"email1\",\"email2\"]")] List<string> emails)
    {
        return await client.AddUsersToRoom(roomId, emails);
    }
}