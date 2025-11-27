using Microsoft.AspNet.SignalR;
using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SyntesisApi
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<int, string> ConnectedUsers = new Dictionary<int, string>();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override Task OnConnected()
        {
            logger.Info("OnConnected - " + DateTime.Now.ToString() + " - " + "Successfully!!");
            var userIdString = Context.QueryString["userId"];
            if (int.TryParse(userIdString, out int userId))
            {
                lock (ConnectedUsers)
                {
                    if (!ConnectedUsers.ContainsKey(userId))
                        ConnectedUsers.Add(userId, Context.ConnectionId);
                    else
                        ConnectedUsers[userId] = Context.ConnectionId; // refresh if reconnected
                }

                Clients.All.UserConnected(userId); // optional broadcast
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            logger.Info("OnDisconnected - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + stopCalled);
            int? userId = null;
            foreach (var kvp in ConnectedUsers)
            {
                if (kvp.Value == Context.ConnectionId)
                {
                    userId = kvp.Key;
                    break;
                }
            }

            if (userId.HasValue)
            {
                ConnectedUsers.Remove(userId.Value);
                Clients.All.UserDisconnected(userId.Value);
                Clients.All.onlineofflinestatus(new { userId = userId.Value, IsOnline = false });
                new BALChat().UpdateUserOnlineOfflineStatus(userId.Value,false);
            }

            return base.OnDisconnected(stopCalled);
        }

        // Register user connection
        public void RegisterUser(int userId)
        {
            logger.Info("RegisterUser - " + DateTime.Now.ToString() + " - " + "Successfully!!" + userId);
            if (ConnectedUsers.ContainsKey(userId))
                ConnectedUsers[userId] = Context.ConnectionId;
            else
                ConnectedUsers.Add(userId, Context.ConnectionId);
            logger.Info("RegisterUser - " + DateTime.Now.ToString() + " - " + "Successfully!!" + userId + "-" + Context.ConnectionId);

            Clients.Caller.UserRegistered(userId);
            Clients.All.onlineofflinestatus(new { userId = userId,IsOnline = true });
            new BALChat().UpdateUserOnlineOfflineStatus(userId, true);
        }

        // Send a private message
        public void SendPrivateMessage(ChatMessageDto message)
        {
            logger.Info($"SendPrivateMessage - {DateTime.Now} - Successfully!! - " + message);

            if (ConnectedUsers.ContainsKey(message.receiverId))
            {
                Clients.Client(ConnectedUsers[message.receiverId])
                       .receiveMessage(message);
                Clients.Client(ConnectedUsers[message.receiverId]).UnreadCountUpdated(message);
            }

            if (ConnectedUsers.ContainsKey(message.senderId))
            {
                Clients.Client(ConnectedUsers[message.senderId])
                       .receiveMessage(message);
            }
        }

        // Send a group message
        public void SendGroupMessage(ChatMessageDto message)
        {
            logger.Info("SendGroupMessage - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            Clients.Group(message.groupId.ToString()).receiveGroupMessage(message);
            var members = new BALChat().GetGroupMembers(message.groupId).Where(id => id != message.senderId);
            foreach (var memberId in members)
            {
                if (ConnectedUsers.ContainsKey(memberId))
                {
                    string connectionId = ConnectedUsers[memberId];
                    Clients.Client(connectionId).UnreadCountUpdated(message);
                }
            }
        }

        // Join group
        public void JoinGroup(ChatMessageDto message)
        {
            logger.Info("JoinGroup - " + DateTime.Now.ToString() + " - " + "Successfully!!" + message);
            Groups.Add(Context.ConnectionId, message.groupId.ToString());
        }

        // Leave group
        public void LeaveGroup(ChatMessageDto message)
        {
            logger.Info("LeaveGroup - " + DateTime.Now.ToString() + " - " + "Successfully!!" + message);
            Groups.Remove(Context.ConnectionId, message.groupId.ToString());
        }

        // Edit message
        public void EditMessage(ChatMessageDto message)
        {
            logger.Info("EditMessage - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            Clients.All.messageEdited(message);
        }

        // Delete message
        public void DeleteMessage(ChatMessageDto message)
        {
            logger.Info("DeleteMessage - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            Clients.All.messageDeleted(message);
        }

        // Add reaction
        public void AddReaction(ChatMessageDto message)
        {
            logger.Info("AddReaction - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            Clients.All.ReactionAdded(message);
        }

        // Remove reaction
        public void RemoveReaction(ChatMessageDto message)
        {
            logger.Info("RemoveReaction - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            Clients.All.ReactionRemoved(message);
        }

        // Edit group
        public void EditGroupChat(ChatMessageDto message)
        {
            logger.Info("EditGroupChat - " + DateTime.Now.ToString() + " - " + "Successfully!!");
            Clients.Group(message.groupId.ToString()).EditGroupChat(message);
        }

        // Update recent chat list
        public void UpdateRecentChatList(ChatMessageDto message)
        {
            logger.Info("UpdateRecentChatList - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message.userId);
            if (ConnectedUsers.ContainsKey(message.userId))
            {
                Clients.Client(ConnectedUsers[message.userId]).RecentChatListUpdated();
            }
        }

        // Update unread count
        public void UpdateUnreadCount(ChatMessageDto message)
        {
            logger.Info("UpdateUnreadCount - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            if (ConnectedUsers.ContainsKey(message.userId))
            {
                Clients.Client(ConnectedUsers[message.userId]).UnreadCountUpdated(message);
            }
        }

        // Group created
        public void GroupCreated(ChatMessageDto message)
        {
            logger.Info("GroupCreated - " + DateTime.Now.ToString() + " - " + "Successfully!!");
            Clients.Group(message.groupId.ToString()).GroupCreated(message);
        }

        // Send Typing Status One to One
        public void SendTypingStatus(ChatMessageDto message)
        {
            logger.Info("SendTypingStatus - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            if (ConnectedUsers.ContainsKey(message.receiverId))
            {
                string connectionId = ConnectedUsers[message.receiverId];
                Clients.Client(connectionId).updateTypingStatus(message);
            }
        }

        // Send Typing Status to entire Group
        public void SendGroupTypingStatus(ChatMessageDto message)
        {
            logger.Info("SendGroupTypingStatus - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            var members = new BALChat().GetGroupMembers(message.groupId).Where(id => id != message.senderId);
            foreach (var memberId in members)
            {
                if (ConnectedUsers.ContainsKey(memberId))
                {
                    string connectionId = ConnectedUsers[memberId];
                    Clients.Client(connectionId).updateGroupTypingStatus(message);
                }
            }
        }

        // Update Message Is Read
        public void UpdateIsReadMessage(ChatMessageDto message)
        {
            logger.Info("UpdateIsReadMessage - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + message);
            if (ConnectedUsers.ContainsKey(message.receiverId))
            {
                Clients.Client(ConnectedUsers[message.receiverId]).updateIsRead(message);
            }

            if (ConnectedUsers.ContainsKey(message.senderId))
            {
                Clients.Client(ConnectedUsers[message.senderId]).updateIsRead(message);
            }
        }

        // Update User Online Offline Status
        public void UserOnlineOfflineStatus(int UserId , bool IsOnline)
        {
            logger.Info("UserOnlineOfflineStatus - " + DateTime.Now.ToString() + " - " + "Successfully!!" + "-" + UserId + "-" + IsOnline);
            Clients.All.onlineofflinestatus(new { userId = UserId, IsOnline = IsOnline });
            new BALChat().UpdateUserOnlineOfflineStatus(UserId, IsOnline);
        }
    }
}