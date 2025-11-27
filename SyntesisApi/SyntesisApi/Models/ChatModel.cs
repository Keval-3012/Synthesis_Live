using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Models
{
    public class CreateGroup
    {
        public string GroupName { get; set; }
        public string UserId { get; set; }
        public string UserIds { get; set; }
        public string AdminUserIds { get; set; }
    }

    public class EditGroup
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public int UserId { get; set; }
        public string UserIds { get; set; }
        public string AdminUserIds { get; set; }
    }

    public class SendMessageRequest
    {
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? GroupId { get; set; }
        public string Message { get; set; }
        public string ConversationId { get; set; }
        public string ConversationType { get; set; } = "Message";
        public string UploadFile { get; set; }
    }

    public class GroupMemberModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
    }

    public class EditMessageRequest
    {
        public int ChatId { get; set; }
        public string Message { get; set; }
    }

    public class DeleteMessageRequest
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
    }

    public class SendReactionRequest
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string ReactionEmoji { get; set; }
    }

    public class GetMessagesRequest
    {
        public string ConversationId { get; set; }
        public int? CurrentUserId { get; set; }
        public int? TargetUserId { get; set; }
    }

    public class SendAttachmentModel
    {
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? GroupId { get; set; }
        public string Message { get; set; }
        public string ConversationId { get; set; }
        public string UploadFile { get; set; }
    }

    public class UpdateIsReadRequest
    {
        public int? ChatId { get; set; }
        public int ReceiverId { get; set; }
        public string ConversationId { get; set; }
        public int? GroupId { get; set; }
    }

    public class TypingStatus
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool IsTyping { get; set; }
    }

    public class GroupTypingStatus
    {
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public bool IsTyping { get; set; }
    }

    public class GetGroupMembers
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<GroupMembers> Members { get; set; }
    }

    public class GroupMembers
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ForwardMessageRequest
    {
        public int SenderId { get; set; }
        public List<int> ReceiverId { get; set; }
        public List<int> GroupId { get; set; }
        public string Message { get; set; }
        public string ConversationId { get; set; }
        public string ConversationType { get; set; } = "Message";
        public string UploadFile { get; set; }
    }

    public class ForwardChatMessageResponse
    {
        public int ReceiverId { get; set; }
        public int ChatID { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ForwardAttechmentResponse
    {
        public int? ChatId { get; set; }
        public string ConversationId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? GroupId { get; set; }
        public string Message { get; set; }
        public DateTime? Timestamp { get; set; }
        public string ConversationType { get; set; }
        public string UploadFile { get; set; }
    }

    public class ChatRecord
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public DateTime? Timestamp { get; set; }
        public string LastMessageFrom { get; set; }
        public int? LastMessageFromId { get; set; }
        public string LastMessage { get; set; }
        public string ChatType { get; set; }
        public int? GroupId { get; set; }
        public string ConversationId { get; set; }
        public bool? IsOnline { get; set; }
        public string IsRead { get; set; }
        public List<UserDto> Users { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ChatMessageDto
    {
        public int chatId { get; set; }
        public int senderId { get; set; }
        public int receiverId { get; set; }
        public int groupId { get; set; }
        public int userId { get; set; }
        public string reactionEmoji { get; set; }
        public string newMessage { get; set; }
        public object messageData { get; set; }
        public DateTime timestamp { get; set; }
        public string groupName { get; set; }
        public int unreadCount { get; set; }
        public bool isTyping { get; set; }
        public bool isonline { get; set; }
        public List<int> memberIds { get; set; }
        public object response { get; set; }

    }

    //public class UpdateUserOnlineOfflineStatus
    //{
    //    public int UserId { get; set; }
    //    public bool IsOnline { get; set; }
    //}
}