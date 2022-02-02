using System.Windows;

namespace VRCRemote
{
    public class JavascriptHandler
    {
        private static readonly string _add_queue = "window.addQueue(\"${queueName}\",${isCreator});";

        private static readonly string _create_message_box = "M.toast({html:\"${message}\"});";

        private static readonly string _change_state = "window.changeState(\"${queueName}\",\"${state}\");";

        private static readonly string _change_overall_state = "window.changeOverallState(\"${state}\");";
        private static readonly string _update_info = "window.updateInfo(\"${username}\",\"${userId}\",\"${instanceId}\");";

        public static string CreateMessageBoxQuery(string message)
        {
            return _create_message_box.Replace("${message}", message);
        }

        public static string CreateChangeStateQuery(string queueName, string state)
        {
            return _change_state.Replace("${queueName}", queueName).Replace("${state}", state);
        }

        public static string CreateAddQueueQuery(string queueName, bool isCreator)
        {
            return _add_queue.Replace("${queueName}", queueName).Replace("${isCreator}", isCreator.ToString().ToLower());
        }

        public static string CreateChangeOverallStateQuery(string state)
        {
            return _change_overall_state.Replace("${state}", state);
        }

        public static string CreateUpdateInfoQuery(string username, string userid, string instanceId)
        {
            return _update_info.Replace("${username}", username).Replace("${userId}", userid)
                .Replace("${instanceId}", instanceId);
        }
    }
}