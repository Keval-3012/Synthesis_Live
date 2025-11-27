using EntityModels.Migrations;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MessageMasterRepository : IMessageMasterRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public MessageMasterRepository(DBContext context)
        {
            _context = context;
        }

        public List<MessageMaster> GetMessageList()
        {
            List<MessageMaster> obj = new List<MessageMaster>();
            try
            {
                obj = _context.MessageMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("MessageMasterRepository - GetMessageList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SaveMessageMaster(int messageid, string keystr, string valueText, string description, string moduleName)
        {
            try
            {
                if (messageid == 0)
                {
                    MessageMaster model = new MessageMaster();
                    model.KeyNum = keystr;
                    model.ValueStr = valueText;
                    model.Description = description;
                    model.ModuleName = moduleName;
                    _context.MessageMasters.Add(model);
                    _context.SaveChanges();
                }
                else
                {
                    MessageMaster model = _context.MessageMasters.Find(messageid);
                    model.KeyNum = keystr;
                    model.ValueStr = valueText;
                    model.Description = description;
                    model.ModuleName = moduleName;
                    _context.Entry(model).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("MessageMasterRepository - SaveMessageMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<string> GetModuleName()
        {
            List<string> obj = new List<string>();
            try
            {
                obj = _context.MessageMasters.Select(m => m.ModuleName).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("MessageMasterRepository - GetModuleName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        
    }
}
