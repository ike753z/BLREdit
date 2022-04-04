using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BLREdit
{
    public class MagiCowClient
    {
        public const string MEDIA_TYPE = "application/json";
        static HttpClient Client { get; set; } = new HttpClient() { BaseAddress = new Uri("http://localhost/"), Timeout = new TimeSpan(0, 0, 10) };

        static MagiCowClient()
        {
            Client.DefaultRequestHeaders.Add("User-Agent", "BLREdit-"+App.CurrentVersion);
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
        }

        public static async Task<MagiCowsProfile[]> GetAllPlayers()
        {
            try
            {
                var response = await Client.GetAsync(
                    $"api/players/all");
                LoggingSystem.Log(response.RequestMessage.ToString());
                LoggingSystem.Log(response.ReasonPhrase);
                return await ConvertMessageBodyToProfiles(response);
            }
            catch(Exception error)
            {
                LoggingSystem.Log("GetAllPlayers Failed:\n"+error.ToString(), LogTypes.Error);
                return null;
            }
        }

        public static async Task<MagiCowsProfile[]> GetOwnedPlayers()
        {
            try
            {
                var response = await Client.GetAsync(
                    $"api/players");
                return await ConvertMessageBodyToProfiles(response);
            }
            catch (Exception error)
            {
                LoggingSystem.Log("GetOwnedPlayer Failed:\n" + error.ToString(), LogTypes.Error);
                return null;
            }
        }

        public static async Task<MagiCowsProfile> GetPlayer(string PlayerName)
        {
            try { 
                var response = await Client.GetAsync(
                    $"api/players?playerName={PlayerName}");
                var players = await ConvertMessageBodyToProfiles(response);
                if (players.Length > 0)
                {
                    return players[0];
                }
                else
                {
                    return new MagiCowsProfile();
                }
            }
            catch (Exception error)
            {
                LoggingSystem.Log("GetOwnedPlayer Failed:\n" + error.ToString(), LogTypes.Error);
                return null;
            }
        }

        public static async Task<bool> PostPlayer(MagiCowsProfile player)
        {
            try
            {
                var response = await Client.PostAsync(
                    $"api/players", new StringContent(IOResources.Serialize(player), Encoding.UTF8, MEDIA_TYPE));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //if(response.)
                    return true;
                }
                else
                {
                    LoggingSystem.Log(player.PlayerName + await response.Content.ReadAsStringAsync(), LogTypes.Error);
                    return false;
                }
            }
            catch(Exception error)
            {
                LoggingSystem.Log(player.PlayerName + " update failed Timeout or other Exception:\n" + error.ToString(), LogTypes.Error);
                return false;
            }
        }

        // TODO: response handling is not finished
        public static async Task<bool> DeletePlayer(MagiCowsProfile player)
        { 
            try
            {
                var response = await Client.DeleteAsync(
                    $"api/players?playerName{player.PlayerName}");
                return true;
            }
            catch (Exception error)
            {
                LoggingSystem.Log("GetOwnedPlayer Failed:\n" + error.ToString(), LogTypes.Error);
                return false;
            }
        }

        static async Task<MagiCowsProfile[]> ConvertMessageBodyToProfiles(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = await response.Content.ReadAsStringAsync();
                return IOResources.Deserialize<MagiCowsProfile[]>(json);
            }
            else
            {
                LoggingSystem.Log(response.Headers.ToString() +  "Request failed with code:" + response.StatusCode.ToString(), LogTypes.Error);
                return null;
            }
        }

        static async Task<MagiCowsProfile> ConvertMessageBodyToProfile(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string json = await response.Content.ReadAsStringAsync();
                return IOResources.Deserialize<MagiCowsProfile>(json);
            }
            else
            {
                LoggingSystem.Log(response.Headers.ToString() + "Request failed with code:" + response.StatusCode.ToString(), LogTypes.Error);
                return null;
            }
        }

    }

    class MagiCowResponse
    {
        public string message { get; set; } = "nothing";
        public string errors { get; set; } = "none";
    }
}
