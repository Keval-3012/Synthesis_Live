using EntityModels.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SynthesisRepo.Controllers
{
    public class QBOperationController : Controller
    {
        private DBContext db = new DBContext();

        #region Old code QBWebHook Direct Dump in DB Table Updated by Dani on 30-10-2025.
        // GET: QBOperation
        //public ActionResult Index()
        //{
        //    WriteErrorLogMsg("Hi I am Baik your friend of ChatGPT");
        //    string InputStream = "";
        //    using (Stream receiveStream = Request.InputStream)
        //    {
        //        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
        //        {
        //            try
        //            {
        //                //string Type = "";
        //                //int StoreID = 0;
        //                //int ID = 0;
        //                InputStream = readStream.ReadToEnd();
        //                InputStream = InputStream.Replace("QB Response Data:", "");
        //                //InputStream = "{\"eventNotifications\":[{\"realmId\":\"9130348819896216\",\"dataChangeEvent\":{\"entities\":[{\"name\":\"Purchase\",\"id\":\"202816\",\"operation\":\"Update\",\"lastUpdated\":\"2022-01-28T20:49:26.000Z\"}]}}]}";
        //                WriteErrorLogMsg(InputStream);
        //                JToken token = JToken.Parse(InputStream);
        //                JArray Receipt = (JArray)token.SelectToken("eventNotifications");
        //                string Name = "";
        //                string Id = "";
        //                int QbOnlineId = 0;
        //                if (Receipt != null)
        //                {
        //                    for (int i = 0; i < Receipt.Count; i++)
        //                    {
        //                        try
        //                        {
        //                            JObject objRow = JObject.Parse(Receipt[i].ToString());
        //                            if (objRow["dataChangeEvent"] != null)
        //                            {
        //                                if (objRow["dataChangeEvent"]["entities"] != null)
        //                                {
        //                                    JArray detail1 = JArray.Parse(objRow["dataChangeEvent"]["entities"].ToString());

        //                                    QBWebhook objwebhook = new QBWebhook();
        //                                    WriteErrorLogMsg("Response" + detail1[0].ToString());
        //                                    JObject objRow1 = JObject.Parse(detail1[0].ToString());
        //                                    WriteErrorLogMsg("realmId" + objRow["realmId"]);
        //                                    objwebhook.RealmId = (objRow["realmId"] == null ? "" : objRow["realmId"].ToString());
        //                                    QbOnlineId = db.QBOnlineConfigurations.Where(w => w.RealmId == objwebhook.RealmId).Select(s => s.QBOnlineId).FirstOrDefault();
        //                                    objwebhook.QBOnlineId = QbOnlineId;
        //                                    objwebhook.Type = (objRow1["name"] == null ? "" : objRow1["name"].ToString());
        //                                    Name = (objRow1["name"] == null ? "" : objRow1["name"].ToString());
        //                                    objwebhook.TXNId = (objRow1["id"] == null ? "" : objRow1["id"].ToString());
        //                                    Id = (objRow1["id"] == null ? "" : objRow1["id"].ToString());
        //                                    objwebhook.Operation = (objRow1["operation"] == null ? "" : objRow1["operation"].ToString());
        //                                    objwebhook.LastUpdated = Convert.ToDateTime(objRow1["lastUpdated"].ToString());
        //                                    db.QBWebhooks.Add(objwebhook);
        //                                    db.SaveChangesAsync();
        //                                }
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            WriteErrorLogMsg("Event Name:" + Name + " " + ex.Message);
        //                        }
        //                    }
        //                }
        //                WriteErrorLogMsg("Event Name:" + Name);
        //                WriteErrorLogMsg("Event ID:" + Id);

        //                WriteErrorLogMsg("QB Response Data:" + InputStream);
        //                //if (Receipt != null)
        //                //{
        //                //    for (int i = 0; i < Receipt.Count; i++)
        //                //    {
        //                //        try
        //                //        {
        //                //            JObject objRow = JObject.Parse(Receipt[i].ToString());
        //                //            if (objRow["dataChangeEvent"] != null)
        //                //            {
        //                //                if (objRow["dataChangeEvent"]["entities"] != null)
        //                //                {
        //                //                    JArray detail1 = JArray.Parse(objRow["dataChangeEvent"]["entities"].ToString());
        //                //                    JObject objRow1 = JObject.Parse(detail1[0].ToString());
        //                //                    StoreID = db.QBOnlineConfigurations.Where(s => s.RealmId == (objRow["realmId"] == null ? "" : objRow["realmId"].ToString())).FirstOrDefault().StoreId;
        //                //                    if (StoreID > 0)
        //                //                    {
        //                //                        Type = (objRow1["name"] == null ? "" : objRow1["name"].ToString());
        //                //                        switch (Type.ToUpper())
        //                //                        {
        //                //                            case "VENDOR":
        //                //                                ID = db.VendorMasters.Where(s => s.ListId == (objRow["id"] == null ? "0" : objRow["id"].ToString()) && s.StoreId == StoreID).FirstOrDefault().VendorId;
        //                //                                if ((objRow1["operation"] == null ? "" : objRow1["operation"].ToString()) == "Update")
        //                //                                {
        //                //                                    var VenData = db.VendorMasters.Find(ID);
        //                //                                    VenData.IsActive = false;
        //                //                                    db.Entry(VenData).State = EntityState.Modified;
        //                //                                    db.SaveChanges();
        //                //                                }
        //                //                                break;
        //                //                            case "ACCOUNT":
        //                //                                ID = db.DepartmentMasters.Where(s => s.ListId == (objRow["id"] == null ? "0" : objRow["id"].ToString()) && s.StoreId == StoreID).FirstOrDefault().DepartmentId;
        //                //                                if ((objRow1["operation"] == null ? "" : objRow1["operation"].ToString()) == "Update")
        //                //                                {
        //                //                                    var DepData = db.DepartmentMasters.Find(ID);
        //                //                                    DepData.IsActive = false;
        //                //                                    db.Entry(DepData).State = EntityState.Modified;
        //                //                                    db.SaveChanges();
        //                //                                }
        //                //                                break;
        //                //                            case "PURCHASE":
        //                //                                ID = db.ExpenseChecks.Where(s => s.TXNId == (objRow["id"] == null ? "0" : objRow["id"].ToString()) && s.StoreId == StoreID).FirstOrDefault().ExpenseCheckId;
        //                //                                if ((objRow1["operation"] == null ? "" : objRow1["operation"].ToString()) == "Delete")
        //                //                                {
        //                //                                    var expData = db.ExpenseChecks.Where(s => s.ExpenseCheckId == ID).FirstOrDefault();
        //                //                                    db.ExpenseChecks.Remove(expData);
        //                //                                    db.SaveChanges();
        //                //                                }
        //                //                                break;
        //                //                        }
        //                //                    }                                            
        //                //                }
        //                //            }
        //                //        }
        //                //        catch (Exception ex)
        //                //        {

        //                //        }
        //                //    }
        //                //}
        //            }
        //            catch (Exception ex)
        //            { }
        //        }
        //    }
        //    return RedirectToAction("Daily", "Dashboard");
        //}
        #endregion

        #region New Code QBWebHook Call To be get saved in files Updated by Dani on 30-10-2025.
        public ActionResult Index()
        {
            try
            {
                WriteErrorLogMsg("QB Webhook Call - Processing incoming webhook");
                string InputStream = "";
                using (Stream receiveStream = Request.InputStream)
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    try
                    {
                        InputStream = readStream.ReadToEnd();
                        InputStream = InputStream.Replace("QB Response Data:", "");
                        WriteErrorLogMsg("Raw webhook data received");

                        // Define the path to the 'Pending' folder inside QBWebHookData
                        string logPath = Server.MapPath("~/QBWebHookData/Pending");

                        // Ensure the 'Pending' directory exists
                        if (!System.IO.Directory.Exists(logPath))
                        {
                            System.IO.Directory.CreateDirectory(logPath);
                        }

                        // Parse JSON to extract all events
                        JToken token = JToken.Parse(InputStream);
                        JArray eventNotifications = (JArray)token.SelectToken("eventNotifications");

                        if (eventNotifications != null && eventNotifications.Count > 0)
                        {
                            int totalFilesCreated = 0;

                            // Loop through each notification
                            for (int i = 0; i < eventNotifications.Count; i++)
                            {
                                try
                                {
                                    JObject notification = JObject.Parse(eventNotifications[i].ToString());

                                    if (notification["dataChangeEvent"] != null && notification["dataChangeEvent"]["entities"] != null)
                                    {
                                        JArray entities = JArray.Parse(notification["dataChangeEvent"]["entities"].ToString());

                                        // Create separate file for EACH entity (webhook)
                                        for (int j = 0; j < entities.Count; j++)
                                        {
                                            try
                                            {
                                                // Create individual webhook with exact QB format
                                                JObject singleWebhook = new JObject();
                                                JArray eventNotificationsArray = new JArray();

                                                JObject singleNotification = new JObject();
                                                singleNotification["realmId"] = notification["realmId"].ToString();

                                                JObject dataChangeEvent = new JObject();
                                                JArray entitiesArray = new JArray();
                                                entitiesArray.Add(entities[j]);
                                                dataChangeEvent["entities"] = entitiesArray;

                                                singleNotification["dataChangeEvent"] = dataChangeEvent;
                                                eventNotificationsArray.Add(singleNotification);
                                                singleWebhook["eventNotifications"] = eventNotificationsArray;

                                                // Create filename with GUID only
                                                string fileName = $"{Guid.NewGuid()}.json";

                                                // Combine path properly
                                                string filePath = System.IO.Path.Combine(logPath, fileName);

                                                // Write individual webhook to separate file (compact format, no indentation)
                                                System.IO.File.WriteAllText(filePath, singleWebhook.ToString(Newtonsoft.Json.Formatting.None));

                                                totalFilesCreated++;

                                                WriteErrorLogMsg($"Webhook saved to file: {fileName}");
                                            }
                                            catch (Exception entityEx)
                                            {
                                                WriteErrorLogMsg($"Error processing entity {j}: " + entityEx.Message);
                                            }
                                        }
                                    }
                                }
                                catch (Exception notificationEx)
                                {
                                    WriteErrorLogMsg($"Error processing notification {i}: " + notificationEx.Message);
                                }
                            }

                            WriteErrorLogMsg($"QB Webhook processing completed. Total files created: {totalFilesCreated}");
                        }
                        else
                        {
                            WriteErrorLogMsg("No event notifications found in webhook data");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteErrorLogMsg("Error processing webhook: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogMsg("QB Webhook error: " + ex.Message);
            }

            // Return 200 OK to acknowledge webhook receipt to QuickBooks
            return new HttpStatusCodeResult(200, "OK");
        }
        #endregion

        public void WriteErrorLogMsg(string Message)
        {

            try
            {
                StreamWriter sw = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath("/WebHookLog/") + "Logs.txt");
                sw.WriteLine(Message);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {

            }
        }
    }
}