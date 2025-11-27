using iTextSharp.text.pdf;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json;
using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using Utility;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Controllers
{
    public class ChatController : ApiController
    {
        //Created by Dani on 28-07-2025

        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Route("api/Chat/GetAllActiveChatListUsers")]
        public async Task<IHttpActionResult> GetAllActiveChatListUsers(int UserId)
        {
            try
            {
                logger.Info("ChatController - GetAllActiveChatListUsers - " + DateTime.Now.ToString());
                Dt1 = new BALChat().GetAllUserList(UserId);

                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "User List got Successfully!!";
                    logger.Info("ChatController - GetAllActiveChatListUsers - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Getting error to load User list!";
                    logger.Error("ChatController - GetAllActiveChatListUsers - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("ChatController - GetAllActiveChatListUsers - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/CreateGroup")]
        public IHttpActionResult CreateGroup([FromBody] CreateGroup createGroup)
        {
            try
            {
                logger.Info("ChatController - CreateGroup - " + DateTime.Now.ToString());
                if (createGroup.GroupName != null && createGroup.UserId != null && createGroup.UserIds != null)
                {
                    var dt = new BALChat().CreateGroup(createGroup);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int groupId = Convert.ToInt32(dt.Rows[0]["GroupId"]);
                        var memberIds = createGroup.UserIds.Split(',').Select(int.Parse).ToList();
                    }

                    Response.responseStatus = "200";
                    Response.responseData = dt;
                    Response.message = "Group Created Successfully!!";
                    logger.Info("ChatController - CreateGroup - " + DateTime.Now + " - Group Created Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Please Pass the required paramter of UserId, UserIds & GroupName!";
                    logger.Error("ChatController - CreateGroup - " + DateTime.Now.ToString() + " - " + "Please Pass the required paramter of UserId, UserIds & GroupName!");

                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("ChatController - CreateGroup - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/EditGroup")]
        public IHttpActionResult EditGroup([FromBody] EditGroup editGroup)
        {
            try
            {
                logger.Info("ChatController - EditGroup - " + DateTime.Now.ToString());
                if (editGroup == null || string.IsNullOrWhiteSpace(editGroup.GroupName))
                {
                    Response.responseStatus = "400";
                    Response.message = "Invalid parameters. Provide GroupName and Members.";
                    return Ok(Response);
                }

                Dt1 = new BALChat().EditGroup(editGroup);

                if (Dt1 != null && Dt1.Rows.Count > 0)
                {
                    var memberIds = editGroup.UserIds?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
                }

                Response.responseStatus = "200";
                Response.responseData = Dt1;
                Response.message = "Group Updated Successfully.";
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.message = ex.Message;
                logger.Error("ChatController - EditGroup - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/SendChatMessage")]
        public IHttpActionResult SendChatMessage([FromBody] SendMessageRequest req)
        {
            try
            {
                logger.Info("ChatController - SendChatMessage - " + DateTime.Now.ToString());
                if (req == null || req.SenderId <= 0 || string.IsNullOrWhiteSpace(req.Message))
                {
                    Response.responseStatus = "400";
                    Response.message = "Invalid parameters.";
                    return Ok(Response);
                }

                if (req.ReceiverId == null && req.GroupId == null)
                {
                    Response.responseStatus = "400";
                    Response.message = "Either ReceiverId or GroupId must be provided.";
                    return Ok(Response);
                }

                List<GroupMemberModel> members;
                DataTable dt;
                int chatId = new BALChat().SendChatMessage(req, out members, out dt);

                if (chatId <= 0)
                {
                    Response.responseStatus = "400";
                    Response.message = "Failed to send message.";
                    return Ok(Response);
                }

                Response.responseStatus = "200";
                Response.message = "Message sent successfully.";
                Response.responseData = dt; // <-- full message row
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.message = ex.Message;
                logger.Error("ChatController - SendChatMessage - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/EditMessage")]
        public IHttpActionResult EditMessage([FromBody] EditMessageRequest req)
        {
            try
            {
                logger.Info("ChatController - EditMessage - " + DateTime.Now.ToString());
                if (req == null || req.ChatId <= 0 || string.IsNullOrWhiteSpace(req.Message))
                {
                    Response.responseStatus = "400";
                    Response.message = "Invalid parameters.";
                    return Ok(Response);
                }

                var dt = new BALChat().EditChatMessage(req.ChatId, req.Message);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.message = "Message updated successfully.";
                    Response.responseData = dt;


                    return Ok(Response);
                }

                Response.responseStatus = "400";
                Response.message = "Message not found or could not be updated.";
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - EditMessage - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/DeleteMessage")]
        public IHttpActionResult DeleteMessage([FromBody] DeleteMessageRequest req)
        {
            try
            {
                logger.Info("ChatController - DeleteMessage - " + DateTime.Now.ToString());
                if (req == null || req.ChatId <= 0 || req.UserId <= 0)
                {
                    Response.responseStatus = "400";
                    Response.message = "Invalid parameters.";
                    return Ok(Response);
                }

                var dt = new BALChat().DeleteChatMessage(req.ChatId, req.UserId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.message = "Message deleted successfully.";
                    Response.responseData = dt;


                    return Ok(Response);
                }

                Response.responseStatus = "400";
                Response.message = "Unable to delete message.";
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - DeleteMessage - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/SendReaction")]
        public IHttpActionResult SendReaction([FromBody] SendReactionRequest req)
        {
            try
            {
                logger.Info("ChatController - SendReaction - " + DateTime.Now.ToString());
                if (req.ChatId <= 0 || req.UserId <= 0)
                {
                    Response.responseStatus = "400";
                    Response.message = "Invalid parameters.";
                    return Ok(Response);
                }

                var dt = new BALChat().SendReaction(req.ChatId, req.UserId, req.ReactionEmoji);
                Response.responseStatus = "200";
                Response.message = req.ReactionEmoji == null ? "Reaction removed." : "Reaction added/updated.";
                Response.responseData = dt;


                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - SendReaction - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/Chat/GetUserReaction")]
        public IHttpActionResult GetUserReaction(int chatId, int userId)
        {
            try
            {
                logger.Info("ChatController - GetUserReaction - " + DateTime.Now.ToString());
                var emoji = new BALChat().GetUserReaction(chatId, userId);
                // Convert string emoji to DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("ReactionEmoji", typeof(string));
                dt.Rows.Add(emoji);
                Response.responseStatus = "200";
                Response.responseData = dt;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - GetUserReaction - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/Chat/GetReactionUsers")]
        public IHttpActionResult GetReactionUsers(int chatId, string emoji = null)
        {
            try
            {
                logger.Info("ChatController - GetReactionUsers - " + DateTime.Now.ToString());
                var dt = new BALChat().GetReactionUsers(chatId, emoji);
                Response.responseStatus = "200";
                Response.responseData = dt;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - GetReactionUsers - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/GetMessages")]
        public IHttpActionResult GetMessages(GetMessagesRequest req)
        {
            try
            {
                logger.Info("ChatController - GetMessages - " + DateTime.Now.ToString());
                var dt = new BALChat().GetMessages(req);
                Response.responseStatus = "200";
                Response.responseData = dt;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - GetMessages - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //[HttpPost]
        //[Route("api/Chat/SendAttachment")]
        //public IHttpActionResult SendAttachment()
        //{
        //    try
        //    {
        //        logger.Info("ChatController - SendAttachment - " + DateTime.Now.ToString());
        //        var httpRequest = HttpContext.Current.Request;

        //        if (httpRequest.Files.Count == 0)
        //            return Ok(new { status = "error", message = "No files uploaded" });

        //        int senderId = Convert.ToInt32(httpRequest.Form["senderId"]);
        //        int? receiverId = string.IsNullOrEmpty(httpRequest.Form["receiverId"]) ? (int?)null : Convert.ToInt32(httpRequest.Form["receiverId"]);
        //        int? groupId = string.IsNullOrEmpty(httpRequest.Form["groupId"]) ? (int?)null : Convert.ToInt32(httpRequest.Form["groupId"]);
        //        long? conversationId = string.IsNullOrEmpty(httpRequest.Form["conversationId"]) ? (long?)null : Convert.ToInt64(httpRequest.Form["conversationId"]);
        //        string message = httpRequest.Form["message"] ?? "";

        //        //==== File Save Path Logic (Same as Upload API) ====
        //        var baseFolderPath = HttpContext.Current.Server.MapPath("~/UserFiles/ChatAttachments");
        //        var currentDate = DateTime.Now;
        //        var yearFolder = currentDate.Year.ToString();
        //        var monthDayFolder = currentDate.ToString("MMdd");
        //        var senderFolder = senderId.ToString();
        //        var fullSavePath = Path.Combine(baseFolderPath, yearFolder, monthDayFolder, senderFolder);

        //        if (!Directory.Exists(fullSavePath))
        //            Directory.CreateDirectory(fullSavePath);

        //        string savedRelativePath = "";
        //        var uploadedFileDetails = new List<object>();
        //        var ReturnFileName = "";

        //        for (int i = 0; i < httpRequest.Files.Count; i++)
        //        {
        //            var file = httpRequest.Files[i];
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var originalFileName = Path.GetFileName(file.FileName);
        //                var fileNameMD = DateTime.Now.ToString("MMddyyyy_HHmmssfff") +
        //                                 "_" + Path.GetFileNameWithoutExtension(originalFileName) +
        //                                 Path.GetExtension(originalFileName);
        //                ReturnFileName = Path.GetFileNameWithoutExtension(originalFileName) + Path.GetExtension(originalFileName);
        //                message = ReturnFileName;

        //                var fileSavePath = Path.Combine(fullSavePath, fileNameMD);
        //                file.SaveAs(fileSavePath);

        //                // Relative path used in DB
        //                savedRelativePath = Path.Combine(yearFolder, monthDayFolder, senderFolder, fileNameMD);

        //                uploadedFileDetails.Add(new
        //                {
        //                    originalName = originalFileName,
        //                    savedName = fileNameMD,
        //                    savedPath = savedRelativePath
        //                });
        //            }
        //        }

        //        //==== Insert into DB & Get ChatId ====
        //        var chatRow = new BALChat().SendAttachment(senderId, receiverId, groupId, message, conversationId, savedRelativePath);

        //        if (chatRow == null)
        //            return Ok(new { status = "error", message = "Failed to save attachment" });

        //        var chatDetails = new
        //        {
        //            ChatId = chatRow["ChatId"],
        //            ConversationId = chatRow["ConversationId"],
        //            SenderId = chatRow["SenderId"],
        //            ReceiverId = chatRow["ReceiverId"],
        //            GroupId = chatRow["GroupId"],
        //            Message = ReturnFileName,
        //            Timestamp = chatRow["Timestamp"],
        //            ConversationType = chatRow["ConversationType"],
        //            UploadFile = chatRow["UploadFile"]
        //        };


        //        return Ok(new
        //        {
        //            status = "success",
        //            message = "Attachment sent successfully",
        //            chatDetails = chatDetails,
        //            fileDetails = uploadedFileDetails
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.responseStatus = "500";
        //        Response.message = ex.Message;
        //        logger.Error("ChatController - SendAttachment - " + DateTime.Now.ToString() + " - " + ex.Message);
        //        return Ok(Response);
        //    }
        //}

        [HttpPost]
        [Route("api/Chat/SendAttachment")]
        public IHttpActionResult SendAttachment()
        {
            try
            {
                logger.Info("ChatController - SendAttachment - " + DateTime.Now.ToString());
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count == 0)
                    return Ok(new { status = "error", message = "No files uploaded" });

                int senderId = Convert.ToInt32(httpRequest.Form["senderId"]);
                int? receiverId = string.IsNullOrEmpty(httpRequest.Form["receiverId"]) ? (int?)null : Convert.ToInt32(httpRequest.Form["receiverId"]);
                int? groupId = string.IsNullOrEmpty(httpRequest.Form["groupId"]) ? (int?)null : Convert.ToInt32(httpRequest.Form["groupId"]);
                string conversationId = string.IsNullOrEmpty(httpRequest.Form["conversationId"]) ? null : httpRequest.Form["conversationId"];

                // File save path logic
                //var baseFolderPath = HttpContext.Current.Server.MapPath("~/UserFiles/ChatAttachments");
                string baseFolderPath = ConfigurationManager.AppSettings["FilePhysicalPath"];
                var currentDate = DateTime.Now;
                var yearFolder = currentDate.Year.ToString();
                var monthDayFolder = currentDate.ToString("MMdd");
                var senderFolder = senderId.ToString();
                var fullSavePath = Path.Combine(baseFolderPath, yearFolder, monthDayFolder, senderFolder);

                if (!Directory.Exists(fullSavePath))
                    Directory.CreateDirectory(fullSavePath);

                var uploadedFileDetails = new List<object>();
                var chatDetailsList = new List<object>();

                // Process each file
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var file = httpRequest.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalFileName = Path.GetFileName(file.FileName);
                        var fileNameMD = DateTime.Now.ToString("MMddyyyy_HHmmssfff") +
                                         "_" + Path.GetFileNameWithoutExtension(originalFileName) +
                                         Path.GetExtension(originalFileName);
                        var returnFileName = Path.GetFileNameWithoutExtension(originalFileName) + Path.GetExtension(originalFileName);
                        var message = returnFileName;

                        var fileSavePath = Path.Combine(fullSavePath, fileNameMD);
                        file.SaveAs(fileSavePath);

                        // Relative path used in DB
                        var savedRelativePath = Path.Combine(yearFolder, monthDayFolder, senderFolder, fileNameMD);

                        // Insert into DB
                        var chatRow = new BALChat().SendAttachment(senderId, receiverId, groupId, message, conversationId, savedRelativePath);

                        if (chatRow == null)
                        {
                            logger.Error("Failed to save attachment for file: " + originalFileName);
                            continue; // Skip this file and continue with others
                        }

                        // Add to response
                        chatDetailsList.Add(new
                        {
                            ChatId = chatRow["ChatId"],
                            ConversationId = chatRow["ConversationId"],
                            SenderId = chatRow["SenderId"],
                            ReceiverId = chatRow["ReceiverId"],
                            GroupId = chatRow["GroupId"],
                            Message = returnFileName,
                            Timestamp = chatRow["Timestamp"],
                            ConversationType = chatRow["ConversationType"],
                            UploadFile = chatRow["UploadFile"]
                        });

                        uploadedFileDetails.Add(new
                        {
                            originalName = originalFileName,
                            savedName = fileNameMD,
                            savedPath = savedRelativePath
                        });
                    }
                }

                if (chatDetailsList.Count == 0)
                    return Ok(new { status = "error", message = "No attachments were saved successfully" });

                return Ok(new
                {
                    status = "success",
                    message = "Attachments sent successfully",
                    chatDetails = chatDetailsList, // Return array of chat details
                    fileDetails = uploadedFileDetails
                });
            }
            catch (Exception ex)
            {
                logger.Error("ChatController - SendAttachment - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(new { status = "error", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/Chat/CopyAttachmentFile")]
        public IHttpActionResult CopyAttachmentFile(int currentUserId, string filePath)
        {
            try
            {
                logger.Info("ChatController - CopyAttachmentFile - " + DateTime.Now.ToString());
                string baseFolderPath = ConfigurationManager.AppSettings["FilePhysicalPath"];

                if (string.IsNullOrEmpty(filePath))
                    return Ok(new { success = false, message = "Invalid file path." });

                // Normalize slashes
                string normalizedFilePath = filePath.Replace('\\', '/');
                string baseFolderSegment = "UserFiles/ChatAttachments";

                // Remove base segment if present
                if (normalizedFilePath.StartsWith(baseFolderSegment, StringComparison.OrdinalIgnoreCase))
                    normalizedFilePath = normalizedFilePath.Substring(baseFolderSegment.Length).TrimStart('/');

                string fullFilePath = Path.Combine(baseFolderPath, normalizedFilePath.Replace('/', '\\'));

                if (!System.IO.File.Exists(fullFilePath))
                    return Ok(new { success = false, message = "Source file not found." });

                // File renaming logic
                var originalFileName = Path.GetFileName(fullFilePath);
                var oldTimestampPattern = @"^\d{8}_\d{9}_*";
                var baseFileName = Regex.Replace(originalFileName, oldTimestampPattern, "");
                if (string.IsNullOrEmpty(baseFileName)) baseFileName = originalFileName;

                var currentDate = DateTime.Now;
                var yearFolder = currentDate.Year.ToString();
                var monthDayFolder = currentDate.ToString("MMdd");
                var recipientFolder = currentUserId.ToString();
                var fullSavePath = Path.Combine(baseFolderPath, yearFolder, monthDayFolder, recipientFolder);

                if (!Directory.Exists(fullSavePath))
                    Directory.CreateDirectory(fullSavePath);

                var fileExt = Path.GetExtension(baseFileName);
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
                var timestamp = currentDate.ToString("MMddyyyy_HHmmssfff");
                var newFileName = $"{timestamp}_{fileNameWithoutExt}{fileExt}";

                var newFilePath = Path.Combine(fullSavePath, newFileName);
                System.IO.File.Copy(fullFilePath, newFilePath, true);

                // Relative DB-friendly path
                var relativePath = Path.Combine(yearFolder, monthDayFolder, recipientFolder, newFileName);

                return Ok(new
                {
                    success = true,
                    message = baseFileName,
                    filePath = relativePath
                });
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - CopyAttachmentFile - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/Chat/GetUnReadChatNotificationCount")]
        public IHttpActionResult GetUnReadChatNotificationCount(int currentUserId)
        {
            try
            {
                logger.Info("ChatController - GetUnReadChatNotificationCount - " + DateTime.Now.ToString());
                int unreadCount = new BALChat().GetUnreadChatCount(currentUserId);

                return Ok(new
                {
                    success = true,
                    count = unreadCount
                });
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - GetUnReadChatNotificationCount - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/SendTypingStatus")]
        public IHttpActionResult SendTypingStatus([FromBody] TypingStatus dto)
        {
            try
            {
                logger.Info("ChatController - SendTypingStatus - " + DateTime.Now.ToString());

                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - SendTypingStatus - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/SendGroupTypingStatus")]
        public IHttpActionResult SendGroupTypingStatus([FromBody] GroupTypingStatus dto)
        {
            try
            {
                logger.Info("ChatController - SendGroupTypingStatus - " + DateTime.Now.ToString());
                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - SendGroupTypingStatus - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/UpdateIsRead")]
        public IHttpActionResult UpdateIsRead(int? chatId, int? receiverId, string conversationId, int? groupId)
        {
            try
            {
                logger.Info("ChatController - UpdateIsRead - " + DateTime.Now.ToString());
                var dt = new BALChat().UpdateIsRead(chatId, receiverId, conversationId, groupId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.message = "Message(s) marked as read successfully";
                    Response.responseData = dt;


                    return Ok(Response);
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - UpdateIsRead - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/Chat/LoadChatRecordList")]
        public IHttpActionResult LoadChatRecordList(int userId)
        {
            try
            {
                logger.Info("ChatController - LoadChatRecordList - " + DateTime.Now.ToString());

                // Step 1: Load chat records as DataTable
                DataTable dt = new BALChat().LoadChatRecordList(userId);

                // Step 2: Convert DataTable → List<ChatRecord>
                List<ChatRecord> chatRecords = Utility.Common.ConvertToList<ChatRecord>(dt);

                // Step 3: Load groups and members
                DataSet groupData = new BALChat().GetGroupWiseMemberList(userId);
                List<GetGroupMembers> lstGroup = new List<GetGroupMembers>();
                List<GroupMembers> lstMember = new List<GroupMembers>();

                if (groupData != null && groupData.Tables != null && groupData.Tables.Count > 0)
                {
                    lstGroup = Utility.Common.ConvertToList<GetGroupMembers>(groupData.Tables[0]);
                    lstMember = Utility.Common.ConvertToList<GroupMembers>(groupData.Tables[1]);

                    foreach (var group in lstGroup)
                    {
                        group.Members = lstMember.Where(x => x.GroupId == group.GroupId).ToList();
                    }
                }

                // Step 4: Attach users only for group chats
                foreach (var chat in chatRecords)
                {
                    if (chat.ChatType == "Group" && chat.GroupId.HasValue)
                    {
                        var group = lstGroup.FirstOrDefault(g => g.GroupId == chat.GroupId);
                        if (group != null)
                        {
                            chat.Users = group.Members.Select(m => new UserDto
                            {
                                UserId = m.UserId,
                                FirstName = m.FirstName,
                                UserName = m.UserName,
                                IsAdmin = m.IsAdmin,
                            }).ToList();
                        }
                    }
                }

                // Step 5: Final response
                Response.responseStatus = "200";
                Response.responseData1 = chatRecords;
                Response.message = null;
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - LoadChatRecordList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/ForwardChatMessage")]
        public IHttpActionResult ForwardChatMessage([FromBody] ForwardMessageRequest req)
        {
            try
            {
                logger.Info("ChatController - ForwardChatMessage - " + DateTime.Now.ToString());
                if (req == null || req.SenderId <= 0 || string.IsNullOrWhiteSpace(req.Message))
                {
                    Response.responseStatus = "400";
                    Response.message = "Invalid parameters.";
                    return Ok(Response);
                }

                if (req.ReceiverId == null && req.GroupId == null)
                {
                    Response.responseStatus = "400";
                    Response.message = "Either ReceiverId or GroupId must be provided.";
                    return Ok(Response);
                }

                List<ForwardChatMessageResponse> chatId = new BALChat().ForwardChatMessage(req);

                if (chatId == null || chatId.Count == 0)
                {
                    Response.responseStatus = "400";
                    Response.message = "Failed to send message.";
                    return Ok(Response);
                }

                Response.responseData = Utility.Common.LINQToDataTable(chatId);
                Response.responseStatus = "200";
                Response.message = "Message sent successfully."; // <-- full message row
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.message = ex.Message;
                logger.Error("ChatController - ForwardChatMessage - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/Chat/ForwardAttachment")]
        public IHttpActionResult ForwardAttachment()
        {
            try
            {
                logger.Info("ChatController - ForwardAttachment - " + DateTime.Now.ToString());
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count == 0)
                    return Ok(new { status = "error", message = "No files uploaded" });

                ForwardMessageRequest req = new ForwardMessageRequest();
                req.SenderId = Convert.ToInt32(httpRequest.Form["senderId"]);
                string ReceiverIds = string.IsNullOrEmpty(httpRequest.Form["receiverId"]) ? (string)null : Convert.ToString(httpRequest.Form["receiverId"]);
                req.ReceiverId = ReceiverIds.Split(',')
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .Select(int.Parse)
                             .ToList();
                //req.ConversationId = string.IsNullOrEmpty(httpRequest.Form["conversationId"]) ? (long?)null : Convert.ToInt64(httpRequest.Form["conversationId"]);
                req.ConversationId = string.IsNullOrEmpty(httpRequest.Form["conversationId"]) ? null : httpRequest.Form["conversationId"];
                string message = httpRequest.Form["message"] ?? "";

                //==== File Save Path Logic (Same as Upload API) ====
                string baseFolderPath = ConfigurationManager.AppSettings["FilePhysicalPath"];
                var currentDate = DateTime.Now;
                var yearFolder = currentDate.Year.ToString();
                var monthDayFolder = currentDate.ToString("MMdd");
                var senderFolder = req.SenderId.ToString();
                var fullSavePath = Path.Combine(baseFolderPath, yearFolder, monthDayFolder, senderFolder);

                if (!Directory.Exists(fullSavePath))
                    Directory.CreateDirectory(fullSavePath);

                string savedRelativePath = "";
                var uploadedFileDetails = new List<object>();

                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var file = httpRequest.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var originalFileName = Path.GetFileName(file.FileName);
                        var fileNameMD = DateTime.Now.ToString("MMddyyyy_HHmmssfff") +
                                         "_" + Path.GetFileNameWithoutExtension(originalFileName) +
                                         Path.GetExtension(originalFileName);

                        var fileSavePath = Path.Combine(fullSavePath, fileNameMD);
                        file.SaveAs(fileSavePath);

                        // Relative path used in DB
                        savedRelativePath = Path.Combine(yearFolder, monthDayFolder, senderFolder, fileNameMD);

                        uploadedFileDetails.Add(new
                        {
                            originalName = originalFileName,
                            savedName = fileNameMD,
                            savedPath = savedRelativePath
                        });
                    }
                }

                //==== Insert into DB & Get ChatId ====
                List<ForwardChatMessageResponse> chatRow = new BALChat().ForwardAttachment(req, savedRelativePath);

                if (chatRow == null)
                    return Ok(new { status = "error", message = "Failed to save attachment" });
                List<ForwardAttechmentResponse> rsp = new List<ForwardAttechmentResponse>();

                rsp = chatRow.Select(x => new ForwardAttechmentResponse
                {
                    ChatId = x.ChatID,
                    ReceiverId = x.ReceiverId,
                    Timestamp = x.Timestamp,
                    ConversationId = req.ConversationId,
                    SenderId = req.SenderId,
                    Message = req.Message,
                    ConversationType = req.ConversationType,
                    UploadFile = req.UploadFile
                }).ToList();


                return Ok(new
                {
                    status = "success",
                    message = "Attachment sent successfully",
                    chatDetails = rsp,
                    fileDetails = uploadedFileDetails
                });
            }
            catch (Exception ex)
            {
                Response.responseStatus = "500";
                Response.message = ex.Message;
                logger.Error("ChatController - ForwardAttachment - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/Chat/GetOneSignalAppKey")]
        public async Task<IHttpActionResult> GetOneSignalAppKey()
        {
            try
            {
                logger.Info("ChatController - GetOneSignalAppKey - " + DateTime.Now.ToString());
                Dt1 = new BALChat().GetOneSignalList();

                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "One Signal App Key got Successfully!!";
                    logger.Info("ChatController - GetOneSignalAppKey - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Getting error to load One Signal App Key!";
                    logger.Error("ChatController - GetOneSignalAppKey - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("ChatController - GetOneSignalAppKey - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(Response);
            }
        }

        //[HttpPost]
        //[Route("api/Chat/ForwardAttachment")]
        //public IHttpActionResult ForwardAttachment()
        //{
        //    try
        //    {
        //        logger.Info("ChatController - ForwardAttachment - " + DateTime.Now.ToString());
        //        var httpRequest = HttpContext.Current.Request;

        //        if (httpRequest.Files.Count == 0)
        //            return Ok(new { status = "error", message = "No files uploaded" });

        //        ForwardMessageRequest req = new ForwardMessageRequest();
        //        req.SenderId = Convert.ToInt32(httpRequest.Form["senderId"]);
        //        string ReceiverIds = string.IsNullOrEmpty(httpRequest.Form["receiverId"]) ? (string)null : Convert.ToString(httpRequest.Form["receiverId"]);
        //        req.ReceiverId = ReceiverIds.Split(',')
        //                     .Where(x => !string.IsNullOrWhiteSpace(x))
        //                     .Select(int.Parse)
        //                     .ToList();
        //        //req.ConversationId = string.IsNullOrEmpty(httpRequest.Form["conversationId"]) ? (long?)null : Convert.ToInt64(httpRequest.Form["conversationId"]);
        //        req.ConversationId = string.IsNullOrEmpty(httpRequest.Form["conversationId"]) ? null : httpRequest.Form["conversationId"];
        //        string message = httpRequest.Form["message"] ?? "";

        //        //==== File Save Path Logic (Same as Upload API) ====
        //        var baseFolderPath = HttpContext.Current.Server.MapPath("~/UserFiles/ChatAttachments");
        //        var currentDate = DateTime.Now;
        //        var yearFolder = currentDate.Year.ToString();
        //        var monthDayFolder = currentDate.ToString("MMdd");
        //        var senderFolder = req.SenderId.ToString();
        //        var fullSavePath = Path.Combine(baseFolderPath, yearFolder, monthDayFolder, senderFolder);

        //        if (!Directory.Exists(fullSavePath))
        //            Directory.CreateDirectory(fullSavePath);

        //        string savedRelativePath = "";
        //        var uploadedFileDetails = new List<object>();

        //        for (int i = 0; i < httpRequest.Files.Count; i++)
        //        {
        //            var file = httpRequest.Files[i];
        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var originalFileName = Path.GetFileName(file.FileName);
        //                var fileNameMD = DateTime.Now.ToString("MMddyyyy_HHmmssfff") +
        //                                 "_" + Path.GetFileNameWithoutExtension(originalFileName) +
        //                                 Path.GetExtension(originalFileName);

        //                var fileSavePath = Path.Combine(fullSavePath, fileNameMD);
        //                file.SaveAs(fileSavePath);

        //                // Relative path used in DB
        //                savedRelativePath = Path.Combine(yearFolder, monthDayFolder, senderFolder, fileNameMD);

        //                uploadedFileDetails.Add(new
        //                {
        //                    originalName = originalFileName,
        //                    savedName = fileNameMD,
        //                    savedPath = savedRelativePath
        //                });
        //            }
        //        }

        //        //==== Insert into DB & Get ChatId ====
        //        List<ForwardChatMessageResponse> chatRow = new BALChat().ForwardAttachment(req, savedRelativePath);

        //        if (chatRow == null)
        //            return Ok(new { status = "error", message = "Failed to save attachment" });
        //        List<ForwardAttechmentResponse> rsp = new List<ForwardAttechmentResponse>();

        //        rsp = chatRow.Select(x => new ForwardAttechmentResponse
        //        {
        //            ChatId = x.ChatID,
        //            ReceiverId = x.ReceiverId,
        //            Timestamp = x.Timestamp,
        //            ConversationId = req.ConversationId,
        //            SenderId = req.SenderId,
        //            Message = req.Message,
        //            ConversationType = req.ConversationType,
        //            UploadFile = req.UploadFile
        //        }).ToList();


        //        return Ok(new
        //        {
        //            status = "success",
        //            message = "Attachment sent successfully",
        //            chatDetails = rsp,
        //            fileDetails = uploadedFileDetails
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.responseStatus = "500";
        //        Response.message = ex.Message;
        //        logger.Error("ChatController - ForwardAttachment - " + DateTime.Now.ToString() + " - " + ex.Message);
        //        return Ok(Response);
        //    }
        //}

        //[HttpPost]
        //[Route("api/Chat/UpdateUserOnlineOfflineStatus")]
        //public IHttpActionResult UpdateUserOnlineOfflineStatus(int UserId, bool IsOnline)
        //{
        //    try
        //    {
        //        logger.Info("ChatController - UpdateUserOnlineOfflineStatus - " + DateTime.Now.ToString());
        //        var dt = new BALChat().UpdateUserOnlineOfflineStatus(UserId, IsOnline);
        //        Response.responseStatus = "200";
        //        Response.responseData = dt;
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.responseStatus = "500";
        //        Response.message = ex.Message;
        //        logger.Error("ChatController - UpdateUserOnlineOfflineStatus - " + DateTime.Now.ToString() + " - " + ex.Message);
        //        return Ok(Response);
        //    }
        //}
    }
}