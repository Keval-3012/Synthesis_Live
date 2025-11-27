using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Repository
{
    public class CheckListExpenseRepository : ICheckListExpenseRepository
    {
        private DBContext db;
        List<CheckIndexList> checkIndexLists = new List<CheckIndexList>();
        CheckIndexList checkIndex = new CheckIndexList();
        Logger logger = LogManager.GetCurrentClassLogger();

        public CheckListExpenseRepository(DBContext context)
        {
            db = context;
        }
        public List<CheckIndexList> Getchecklistbinddata(int StoreId)
        {
            try
            {
                checkIndexLists = db.Database.SqlQuery<CheckIndexList>("SP_CheckList_Proc @Mode={0},@MailSent={1},@StartDate={2},@EndDate={3},@StoreId={4},@SearchText={5}", "CheckListFilter_ALL", "All", null, null, StoreId, "").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseRepository - Getchecklistbinddata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return checkIndexLists;
        }
        public List<CheckIndexList> UpdateMailOutStatus(string id, string value, int UserId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_CheckList_Proc @Mode = {0},@CheckListId = {1},@MailOut={2},@UserID={3}", "UpdateMailOutStatus", id, value, UserId);
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseRepository - UpdateMailOutStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return checkIndexLists;
        }
        public List<CheckIndexList> UpdateMailOutAllStatus(string StartDate, string EndDate, string value, int UserId, string Searchtext, string Mail, int StoreId, string CheckFilter)
        {
            try
            {
                bool val = Convert.ToBoolean(value);
                db.Database.ExecuteSqlCommand("SP_CheckList_Proc @Mode = {0},@StartDate = {1},@EndDate = {2},@MailOut={3},@UserID={4},@SearchText={5},@MailSent={6},@StoreId={7},@CheckFilter={8}", "UpdateMailOutAllStatus", StartDate, EndDate, val, UserId, Searchtext, Mail, StoreId, CheckFilter);
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseRepository - UpdateMailOutStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return checkIndexLists;
        }
        public List<CheckIndexList> CheckListFilter_ALL(string Mail, string startdate, string enddate, int? StoreIds, string Searchtext, string CheckFilter)
        {
            try
            {
                checkIndexLists = db.Database.SqlQuery<CheckIndexList>("SP_CheckList_Proc @Mode={0},@MailSent={1},@StartDate={2},@EndDate={3},@StoreId={4},@SearchText={5},@CheckFilter={6}", "CheckListFilter_Search", Mail.ToString().Replace("^", "&"), startdate == "" ? null : startdate, enddate == "" ? null : enddate, StoreIds, Searchtext == null ? "" : Searchtext, CheckFilter.ToString().Replace("^", "&")).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CheckListExpenseRepository - CheckListFilter_ALL - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return checkIndexLists;
        }
    }
}
