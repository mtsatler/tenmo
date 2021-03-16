using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        //Transfer RequestTransfer(int transferTypeId, int transferStatusId, int fromAccount, int toAccount, decimal amount);
        Transfer CreateTransfer(int transferTypeId, int transferStatusId, int fromAccount, int toAccount, decimal amount);
        List<Transfer> GetTransfersByUser(int userId);
        TransferStatus GetTransferStatusById(int statusId);
        TransferType GetTransferTypeById(int typeId);
        Transfer GetTransfer(int id);
        Transfer UpdateTransfer(Transfer transfer);
    }
}
