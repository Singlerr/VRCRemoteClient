using System;
using System.Threading.Tasks;
using System.Windows;
using WebSocketSharp;

namespace VRCRemote
{
    public class ConnectionHandler
    {
        private static readonly string _websocketUrl = "ws://reomotevrc.kro.kr:8080/cloud";

        private static ConnectionHandler _instance;
        private string _userId;
        private string _username;

        private Task _updatingTask;
        public bool Initialized;

        private WebSocket _webSocket;

        private ConnectionHandler()
        {
        }


        public static ConnectionHandler GetInstance()
        {
            if (_instance == null)
                return _instance = new ConnectionHandler();
            return _instance;
        }

        public void Initialize(string username, string userId)
        {
            _username = username;
            _userId = userId;
            _webSocket = new WebSocket(_websocketUrl);
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnClose += OnClose;
            _webSocket.OnError += OnError;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.ConnectAsync();
            if(_updatingTask == null)
                _updatingTask = VRChatHandler.StartUpdatingTask();
        }


        private void OnOpen(object sender, EventArgs e)
        {
            var data = DataPacket.CreateHandshakePacket(_username, _userId);
            _webSocket.SendData(data, null);
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            var query = JavascriptHandler.CreateMessageBoxQuery("<span>서버와 연결이 끊겼습니다.</span><button class='btn-flat toast-action' onclick='retryConnection();'>재시도</button>");
            Execute(query);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            var query = JavascriptHandler.CreateMessageBoxQuery("<span>서버와 연결이 끊겼습니다.</span><button class='btn-flat toast-action' onclick='retryConnection();'>재시도</button>");
            Execute(query);
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            var rawData = e.Data;

            if (PacketSerializer.TryDeserialize(rawData, out ReadOnly result))
            {
                if (result.Message == null)
                    return;
                switch (result.StatusCode)
                {
                    case (int)IHttpStatusCode.OK:
                        if (result.Message.Equals("normal", StringComparison.InvariantCultureIgnoreCase) ||
                            result.Message.Equals("creator", StringComparison.InvariantCultureIgnoreCase) ||
                            result.Message.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
                            Execute(JavascriptHandler.CreateChangeOverallStateQuery(result.Message));
                        else
                            ShowMessageBox("해당 작업이 완료되었습니다.");
                        break;
                    case (int)IHttpStatusCode.QUEUE_UPDATE:
                        if (PacketSerializer.TryDeserialize(result.Message, out Container[] containers))
                            foreach (var container in containers)
                                CreateQueue(container.QueueName, container.Creator.Equals(_userId));
                        break;
                    case (int)IHttpStatusCode.REQ_INVITE_REQ:
                        var userId = result.Message;
                        var task = VRChatHandler.SendInviteRequest(userId);
                        var awaiter = task.GetAwaiter();
                        awaiter.OnCompleted(
                            () => { ShowMessageBox(task.HandleTaskResult(userId,_userId, _webSocket).Message); });
                        break;
                    default:
                        ShowMessageBox($"해당 작업을 처리할 수 없습니다. 오류 메시지 : {result.Message}");
                        break;
                }
            }
        }


        public void SendReqInviteReq(string userId, string queueName)
        {
            _webSocket.SendData(ActionPacket.CreateActionPacket(userId,  VRChatHandler.Username,Action.ReqInviteReq, new[] { queueName }),
                null);
        }

        public void UpdateInfo(string username, string userId, string instanceId)
        {
            Execute(JavascriptHandler.CreateUpdateInfoQuery(username,userId,instanceId));
        }
        private void ShowMessageBox(string message)
        {
            Execute(JavascriptHandler.CreateMessageBoxQuery(message));
        }

        public void JoinQueue(string queueName, string joinCode)
        {
            _webSocket.SendData(ActionPacket.CreateActionPacket(VRChatHandler.UserId,VRChatHandler.UserId,Action.JoinQueue,new []{queueName,joinCode}),null);
        }
        public void LeaveQueue(string queueName)
        {
            _webSocket.SendData(ActionPacket.CreateActionPacket(VRChatHandler.UserId,VRChatHandler.UserId,Action.LeaveQueue,new []{queueName}),null);
        }
        public void CreateNewQueue(string queue,string joinCode)
        {
            _webSocket.SendData(
                ActionPacket.CreateActionPacket(VRChatHandler.UserId,  VRChatHandler.Username,Action.CreateQueue, new[] { queue,joinCode }), null);
        }

        public void DeleteQueue(string queue)
        {
            _webSocket.SendData(
                ActionPacket.CreateActionPacket(VRChatHandler.UserId,  VRChatHandler.Username,Action.DeleteQueue, new[] { queue }), null);
        }

        private void CreateQueue(string queue, bool isCreator)
        {
            Execute(JavascriptHandler.CreateAddQueueQuery(queue, isCreator));
        }

        private void Execute(string javascript)
        {
            MainWindow.GetInstance().ExecuteJavascript(javascript);
        }
    }

    public static class WebSocketExt
    {
        public static void SendData(this WebSocket webSocket, object data, Action<bool> callback)
        {
            webSocket.SendAsync(PacketSerializer.Serialize(data), callback);
        }

        public static ReadOnly HandleTaskResult(this Task<ReadOnly> task, string requestedUserId, string userId, WebSocket webSocket)
        {
            if (task.IsFaulted)
            {
                webSocket.SendData(new ReadOnly($"{requestedUserId}|{userId}", (int)IHttpStatusCode.REQ_FAIELD), null);
                return new ReadOnly(task.Exception?.Message, (int)IHttpStatusCode.REQ_FAIELD);
            }

            return new ReadOnly("성공적으로 요청을 보냈습니다", (int)IHttpStatusCode.OK);
        }
    }
}