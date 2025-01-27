using System.Net.Http.Json;
using DocSpaceSemanticExample.Api.Models;

namespace DocSpaceSemanticExample.Api;

public class DocSpaceClient(HttpClient httpClient)
{
    private const string ApiEndpoint = "api/2.0";

    public async Task<RoomResponse> CreateRoomAsync(string title, RoomType roomType)
    {
        const string exceptionMessage = "Failed to create room";
        
        var request = new CreateRoomRequest
        {
            Title = title,
            RoomType = roomType
        };

        var response = await httpClient.PostAsJsonAsync($"{ApiEndpoint}/files/rooms", request);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(exceptionMessage);
        }
        
        var content = await response.Content.ReadFromJsonAsync<BaseResponse<RoomResponse>>();
        if (content != null)
        {
            return content.Response;
        }
        
        throw new Exception(exceptionMessage);
    }

    public async Task<bool> AddUsersToRoom(int roomId, List<string> emails)
    {
        var request = new AddUsersToRoomRequest
        {
            Invitations = emails.Select(email => new Invitation { Email = email, Access = 2 })
        };

        try
        {
            var response = await httpClient.PutAsJsonAsync($"{ApiEndpoint}/files/rooms/{roomId}/share", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}