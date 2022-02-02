using System.Threading.Tasks;
using System.Windows;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRCRemote
{
    public class VRChatHandler
    {
        public static string UserId;
        public static string Username;
        public static string InstanceId;

        public static bool Initialized;

        private static AuthenticationApi _authApi;
        private static UsersApi _usersApi;
        private static InviteApi _inviteApi;
        private static NotificationsApi _notificationsApi;

        private static Configuration _configuration;

        private VRChatHandler()
        {
        }


        public static async Task<ReadOnly> Initialize(string username, string password)
        {
            _configuration = new Configuration
            {
                Username = username,
                Password = password
            };

            _authApi = new AuthenticationApi(_configuration);
            
            try
            {
                var user = await _authApi.GetCurrentUserAsync();
                UserId = user.Id;
                Username = user.Username;
                
            }
            catch (ApiException ex)
            {
                return new ReadOnly("로그인에 실패했습니다.", (int)IHttpStatusCode.LOGIN_FAILED);
            }

            _inviteApi = new InviteApi(_configuration);
            Initialized = true;
            return new ReadOnly("로그인 성공", (int)IHttpStatusCode.OK);
        }

        public static async Task<ReadOnly> SendInviteRequest(string userId)
        {
            try
            {
                await _inviteApi.RequestInviteAsync(userId, new RequestInviteRequest());
                return new ReadOnly("초대 요청에 성공했습니다.", (int)IHttpStatusCode.OK);
            }
            catch (ApiException ex)
            {
                return new ReadOnly($"초대 요청에 실패했습니다. 원인: {ex.Message}", (int)IHttpStatusCode.REQ_FAIELD);
            }
        }

        public static async Task StartUpdatingTask()
        {
            _usersApi = new UsersApi(_configuration);
            while (true)
            {
                InstanceId = (await _usersApi.GetUserAsync(UserId)).Location;
                ConnectionHandler.GetInstance().UpdateInfo(Username, UserId, InstanceId);
                await Task.Delay(3000);
            }
        }
    }
}