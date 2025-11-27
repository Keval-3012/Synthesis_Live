using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ItemMovementRepository : IItemMovementRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();

        public ItemMovementRepository(DBContext context)
        {
            db = context;
        }
        public List<ItemMovementdatehistorySelect> GetItemMovementdatehistorySelects(int StoreIds)
        {
            List<ItemMovementdatehistorySelect> itemMovement = new List<ItemMovementdatehistorySelect>();
            try
            {
                itemMovement = db.Database.SqlQuery<ItemMovementdatehistorySelect>("SP_ItemMovementBySupplier @Mode = {0},@StoreId={1}", "SelectItemMovementdatehistorySelect", StoreIds).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementRepository - GetItemMovementdatehistorySelects - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return itemMovement;
        }
        public void GetDeleteItemMovement(ItemMovementViewModal itemMovementViewModal)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_ItemMovementBySupplier @Mode = {0},@ItemMovementHistoryID = {1}", "DELETE", itemMovementViewModal.ItemMovementID);
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementRepository - GetDeleteItemMovement - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void GetMoveItemMovement(ItemMovementdatehistory itemMovementdatehistory)
        {
            try
            {
                db.ItemMovementdatehistory.Add(itemMovementdatehistory);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementRepository - GetMoveItemMovement - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<ItemMovementBySupplierSelect> SelectItemMovement(object value)
        {
            List<ItemMovementBySupplierSelect> itemMovement = new List<ItemMovementBySupplierSelect>();
            try
            {
                    itemMovement = db.Database.SqlQuery<ItemMovementBySupplierSelect>("SP_ItemMovementBySupplier @Mode = {0},@ItemMovementHistoryID={1}", "SelectItemMovement", value).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ItemMovementRepository - SelectItemMovement - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return itemMovement;
        }
    }
}
