using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IItemMovementRepository
    {
        List<ItemMovementdatehistorySelect> GetItemMovementdatehistorySelects(int StoreIds);
        void GetDeleteItemMovement(ItemMovementViewModal itemMovementViewModal);
        void GetMoveItemMovement(ItemMovementdatehistory itemMovementdatehistory);
        List<ItemMovementBySupplierSelect> SelectItemMovement(object Value);
    }
}
