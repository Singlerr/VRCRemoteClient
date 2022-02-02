using System;
using Newtonsoft.Json;

namespace VRCRemote
{
    public class ActionPacket : DataPacket
    {
        [JsonProperty("action")] private string _action;

        [JsonProperty("params")] private string[] _args;

        public ActionPacket(string userId, string username, string action = "",
            string[] args = null) : base(
            username, userId)
        {
            _action = action;
            _args = args;
        }

        public static ActionPacket CreateActionPacket(string userId,string username, Action action, string[] args = null)
        {
            return new ActionPacket(userId, username,action.GetActionString(), args);
        }
    }

    public enum Action
    {
        [Action("CREATE_QUEUE")] CreateQueue,
        [Action("DELETE_QUEUE")] DeleteQueue,
        [Action("REQ_INVITE_REQ")] ReqInviteReq,
        [Action("JOIN_QUEUE")] JoinQueue,
        [Action("QUIT_QUEUE")] LeaveQueue
    }

    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string action)
        {
            Action = action;
        }

        public string Action { get; protected set; }
    }

    public static class ActionAttributeExt
    {
        public static string GetActionString(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes(
                typeof(ActionAttribute), false) as ActionAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].Action : null;
        }
    }
}