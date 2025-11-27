using SyntesisApi.DAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using static SyntesisApi.Models.ApiModel;
using System.IO;
using System.Net.Http;
using System.Configuration;
using System.Web.Http;
using System.Web.Razor.Tokenizer;
using System.Globalization;
using System.Text.RegularExpressions;
using NLog;
using Org.BouncyCastle.Ocsp;

namespace SyntesisApi.BAL
{
    public class BALChat
    {
        //Created by Dani on 28-07-2025

        clsDAL db = new clsDAL();
        public DataTable GetAllUserList(int UserId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllActiveUserLists");
                Cmd.Parameters.AddWithValue("@UserId", UserId);
                dt = new DataTable();
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
            }
            return dt;
        }

        public DataTable CreateGroup(CreateGroup createGroup)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "CreateGroup");
                Cmd.Parameters.AddWithValue("@GroupName", createGroup.GroupName);
                Cmd.Parameters.AddWithValue("@UserId", createGroup.UserId);
                Cmd.Parameters.AddWithValue("@UserIds", createGroup.UserIds);
                Cmd.Parameters.AddWithValue("@AdminUserIds", createGroup.AdminUserIds);
                dt = new DataTable();
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
            }
            return dt;
        }

        public DataTable EditGroup(EditGroup editGroup)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "EditGroup");
                Cmd.Parameters.AddWithValue("@GroupId", editGroup.GroupId ?? 0);
                Cmd.Parameters.AddWithValue("@GroupName", editGroup.GroupName ?? "");
                Cmd.Parameters.AddWithValue("@UserId", editGroup.UserId);
                Cmd.Parameters.AddWithValue("@UserIds", editGroup.UserIds ?? "");
                Cmd.Parameters.AddWithValue("@AdminUserIds", editGroup.AdminUserIds ?? "");
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
            }
            return dt;
        }

        public int SendChatMessage(SendMessageRequest req, out List<GroupMemberModel> groupMembers, out DataTable dt)
        {
            groupMembers = new List<GroupMemberModel>();
            dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "SendMessage");
                Cmd.Parameters.AddWithValue("@SenderId", req.SenderId);
                Cmd.Parameters.AddWithValue("@ReceiverId", (object)req.ReceiverId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@GroupId", (object)req.GroupId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@Message", req.Message);
                Cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                Cmd.Parameters.AddWithValue("@ConversationId", req.ConversationId ?? null);
                Cmd.Parameters.AddWithValue("@ConversationType", req.ConversationType);
                Cmd.Parameters.AddWithValue("@UploadFile", (object)req.UploadFile ?? DBNull.Value);

                dt = db.Select_New(Cmd);

                int chatId = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    chatId = Convert.ToInt32(dt.Rows[0]["ChatId"]);
                }

                // If group, fetch members
                if (req.GroupId != null && req.GroupId > 0)
                {
                    groupMembers = GetGroupMembersInternal(req.GroupId.Value);
                }

                return chatId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private List<GroupMemberModel> GetGroupMembersInternal(int groupId)
        {
            List<GroupMemberModel> members = new List<GroupMemberModel>();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetGroupMembers");
                Cmd.Parameters.AddWithValue("@GroupId", groupId);

                DataTable dt = db.Select_New(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        members.Add(new GroupMemberModel
                        {
                            UserId = Convert.ToInt32(row["UserId"]),
                            FirstName = Convert.ToString(row["FirstName"])
                        });
                    }
                }
            }
            catch (Exception)
            {
            }
            return members;
        }

        public DataTable EditChatMessage(int chatId, string message)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "EditMessage");
                Cmd.Parameters.AddWithValue("@ChatId", chatId);
                Cmd.Parameters.AddWithValue("@Message", message);
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
            }
            return dt;
        }

        public DataTable DeleteChatMessage(int chatId, int userId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "DeleteMessage");
                Cmd.Parameters.AddWithValue("@ChatId", chatId);
                Cmd.Parameters.AddWithValue("@UserId", userId);
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {

            }
            return dt;
        }

        public DataTable SendReaction(int chatId, int userId, string reactionEmoji)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "SendReaction");
                Cmd.Parameters.AddWithValue("@ChatId", chatId);
                Cmd.Parameters.AddWithValue("@UserId", userId);
                // Use Unicode-safe NVARCHAR parameter for emojis
                Cmd.Parameters.Add("@ReactionEmoji", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(reactionEmoji) ? DBNull.Value : (object)reactionEmoji;

                dt = db.Select_New(Cmd); // Always returns DataTable
            }
            catch (Exception)
            {
            }
            return dt;
        }

        public string GetUserReaction(int chatId, int userId)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetUserReaction");
                Cmd.Parameters.AddWithValue("@ChatId", chatId);
                Cmd.Parameters.AddWithValue("@UserId", userId);
                var dt = db.Select_New(Cmd);
                return dt != null && dt.Rows.Count > 0 ? dt.Rows[0]["ReactionEmoji"].ToString() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetReactionUsers(int chatId, string reactionEmoji)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetReactionUsers");
                Cmd.Parameters.AddWithValue("@ChatId", chatId);
                Cmd.Parameters.AddWithValue("@ReactionEmoji", (object)reactionEmoji ?? DBNull.Value);
                return db.Select_New(Cmd);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetMessages(GetMessagesRequest req)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetMessages");
                Cmd.Parameters.AddWithValue("@ConversationId", req.ConversationId ?? (object)DBNull.Value);
                Cmd.Parameters.AddWithValue("@CurrentUserId", req.CurrentUserId ?? (object)DBNull.Value);
                Cmd.Parameters.AddWithValue("@TargetUserId", req.TargetUserId ?? (object)DBNull.Value);
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
                return null;
            }
            return dt;
        }

        public DataTable UpdateUserOnlineOfflineStatus(int UserId, bool IsOnline)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "UpdateUserOnlineOfflineStatus");
                Cmd.Parameters.AddWithValue("@UserId", UserId);
                Cmd.Parameters.AddWithValue("@IsOnline", IsOnline);
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
                return null;
            }
            return dt;
        }

        public DataRow SendAttachment(int senderId, int? receiverId, int? groupId, string message, string conversationId, string savedRelativePath)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "SendAttachment");
                Cmd.Parameters.AddWithValue("@SenderId", senderId);
                Cmd.Parameters.AddWithValue("@ReceiverId", (object)receiverId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@GroupId", (object)groupId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@Message", message);
                Cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                Cmd.Parameters.AddWithValue("@ConversationId", (object)conversationId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@ConversationType", "File");
                Cmd.Parameters.AddWithValue("@UploadFile", savedRelativePath);

                DataTable dt = db.Select_New(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0];
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetUserName(int userId)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetUserName");
                Cmd.Parameters.AddWithValue("@UserId", userId);

                DataTable dt = db.Select_New(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0]["FirstName"].ToString();
            }
            catch (Exception)
            {
            }
            return "Unknown";
        }

        public List<int> GetGroupMembers(int groupId)
        {
            List<int> members = new List<int>();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetGroupMembers");
                Cmd.Parameters.AddWithValue("@GroupId", groupId);

                DataTable dt = db.Select_New(Cmd);
                foreach (DataRow row in dt.Rows)
                    members.Add(Convert.ToInt32(row["UserId"]));
            }
            catch (Exception)
            {
            }
            return members;
        }

        public int GetUnreadChatCount(int currentUserId)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetUnreadChatCount");
                Cmd.Parameters.AddWithValue("@UserId", currentUserId);

                DataTable dt = db.Select_New(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                    return Convert.ToInt32(dt.Rows[0]["UnreadCount"]);
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public DataTable UpdateIsRead(int? chatId, int? receiverId, string conversationId, int? groupId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "UpdateIsRead");
                Cmd.Parameters.AddWithValue("@ChatId", (object)chatId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@ReceiverId", (object)receiverId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@ConversationId", (object)conversationId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@GroupId", (object)groupId ?? DBNull.Value);

                dt = db.Select_New(Cmd);
            }
            catch
            {
                return null;
            }
            return dt;
        }

        public DataTable LoadChatRecordList(int userId)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "LoadChatRecordList");
                Cmd.Parameters.AddWithValue("@UserId", userId);

                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
            }
            return dt;
        }

        public DataSet GetGroupWiseMemberList(int userId)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetGroupMembersbyUserID");
                Cmd.Parameters.AddWithValue("@UserId", userId);

                ds = db.SelectDataSet(Cmd);
            }
            catch (Exception)
            {
            }
            return ds;
        }

        public List<ForwardChatMessageResponse> ForwardChatMessage(ForwardMessageRequest req)
        {
            List<ForwardChatMessageResponse> dts = new List<ForwardChatMessageResponse>();
            try
            {
                if (req.ReceiverId != null && req.ReceiverId.Count > 0)
                {
                    for (int i = 0; i < req.ReceiverId.Count; i++)
                    {
                        dts.Add(new ForwardChatMessageResponse() { ChatID = this.ForwardChatMessagebyID(req, req.ReceiverId[i]), ReceiverId = req.ReceiverId[i] });
                    }
                }
            }
            catch (Exception)
            {
            }
            return dts;
        }

        public int ForwardChatMessagebyID(ForwardMessageRequest req, int ReceiverId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "ForwardChatMessage");
                Cmd.Parameters.AddWithValue("@SenderId", req.SenderId);
                Cmd.Parameters.AddWithValue("@ReceiverId", (object)ReceiverId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@Message", req.Message);
                Cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                Cmd.Parameters.AddWithValue("@ConversationId", req.ConversationId ?? null);
                Cmd.Parameters.AddWithValue("@ConversationType", req.ConversationType);
                Cmd.Parameters.AddWithValue("@UploadFile", (object)req.UploadFile ?? DBNull.Value);

                dt = db.Select_New(Cmd);

                int chatId = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    chatId = Convert.ToInt32(dt.Rows[0]["ChatId"]);
                }

                return chatId;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<ForwardChatMessageResponse> ForwardAttachment(ForwardMessageRequest req, string savedRelativePath)
        {
            List<ForwardChatMessageResponse> dts = new List<ForwardChatMessageResponse>();
            try
            {
                if (req.ReceiverId != null && req.ReceiverId.Count > 0)
                {
                    for (int i = 0; i < req.ReceiverId.Count; i++)
                    {
                        dts.Add(this.ForwardAttachmentbyID(req, savedRelativePath, req.ReceiverId[i]));
                    }
                }
            }
            catch (Exception)
            {
            }
            return dts;
        }

        public ForwardChatMessageResponse ForwardAttachmentbyID(ForwardMessageRequest req, string savedRelativePath, int receiverId)
        {
            ForwardChatMessageResponse ds = new ForwardChatMessageResponse();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "SendAttachment");
                Cmd.Parameters.AddWithValue("@SenderId", req.SenderId);
                Cmd.Parameters.AddWithValue("@ReceiverId", (object)receiverId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@Message", req.Message);
                Cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                Cmd.Parameters.AddWithValue("@ConversationId", (object)req.ConversationId ?? DBNull.Value);
                Cmd.Parameters.AddWithValue("@ConversationType", "File");
                Cmd.Parameters.AddWithValue("@UploadFile", savedRelativePath);
                DataTable dt = db.Select_New(Cmd);
                ds = Utility.Common.ConvertToList<ForwardChatMessageResponse>(dt).FirstOrDefault();
                ds.ReceiverId = receiverId;
            }
            catch (Exception)
            {
            }
            return ds;
        }

        public DataTable GetOneSignalList()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ChatMessenger_API";
                Cmd.Parameters.AddWithValue("@Mode", "GetOneSignalCredentials");
                dt = new DataTable();
                dt = db.Select_New(Cmd);
            }
            catch (Exception)
            {
            }
            return dt;
        }
    }
}