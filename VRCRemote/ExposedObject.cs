using System.Threading.Tasks;
using System.Windows;
using CefSharp;

namespace VRCRemote
{
    public class ExposedObject
    {
        public string GetCurrentInstanceId()
        {
            return VRChatHandler.InstanceId;
        }

        public string GetUsername()
        {
            return VRChatHandler.Username;
        }

        public string GetUserId()
        {
            return VRChatHandler.UserId;
        }

        public async Task<ReadOnly> SendInviteRequest(string userId)
        {
            return await VRChatHandler.SendInviteRequest(userId);
        }

        public void JoinQueue(string queueName, string joinCode)
        { 
            ConnectionHandler.GetInstance().JoinQueue(queueName, joinCode);
        }

        public void LeaveQueue(string queueName)
        {
            ConnectionHandler.GetInstance().LeaveQueue(queueName);
        }

        public void RequestInviteRequest(string queueName, string userId)
        {
            ConnectionHandler.GetInstance().SendReqInviteReq(userId, queueName);
        }

        public void CreateQueue(string queueName, string joinCode)
        {
            ConnectionHandler.GetInstance().CreateNewQueue(queueName,joinCode);
        }

        public void DeleteQueue(string queueName)
        {
            ConnectionHandler.GetInstance().DeleteQueue(queueName);
        }

        public void StartConnection()
        {
            ConnectionHandler.GetInstance().Initialize(VRChatHandler.Username, VRChatHandler.UserId);
        }

        public ReadOnly Initialize(string username, string password)
        {
            //VRChatHandler.Initialize(username, password).Result;
            return VRChatHandler.Initialize(username, password).Result;
        }

        public void GoToMainPage()
        {
            MainWindow.GetInstance().LoadUrl("local://resources/index.html");
        }
    }
}