using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private static ITransferDAO transferDao;
        private static IUserDAO userDao;

        public TransferController(ITransferDAO _transferDao, IUserDAO _userDao)
        {
            transferDao = _transferDao;
            userDao = _userDao;
        }
    
        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransfer(int transferId)
        {
            Transfer transfer = transferDao.GetTransfer(transferId);

            if (transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("users/{userId}")]
        public ActionResult<List<Transfer>> GetUserTransfers(int userId)
        {
            List<Transfer> transfers = transferDao.GetTransfersByUser(userId);

            if(transfers != null)
            {
                return Ok(transfers);

            }
            else
            {
                return NotFound();
            }

        }
        [HttpPost]
        public ActionResult<Transfer> AddTransfer(Transfer transfer)
        {
            Transfer added = transferDao.CreateTransfer(transfer.transfer_type_id, transfer.transfer_status_id, transfer.account_from, transfer.account_to, transfer.amount);
            return Created($"/transfer/{added.transfer_id}", added);
        }

        [HttpPut("{transferId}")]
        public ActionResult<Transfer> UpdateTransfer(Transfer transfer)
        {
            Transfer updated = transferDao.UpdateTransfer(transfer);
            return Ok(updated);
        }

        [HttpGet("status/{transferId}")]
        public ActionResult<TransferStatus> GetTransferStatusById(int transferId)
        {
            TransferStatus transferStatus = transferDao.GetTransferStatusById(transferId);

            if (transferStatus != null)
            {
                return Ok(transferStatus);

            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("type/{transferId}")]
        public ActionResult<TransferType> GetTransferTypeById(int transferId)
        {
            TransferType transferType = transferDao.GetTransferTypeById(transferId);

            if(transferType != null)
            {
                return Ok(transferType);
            }
            else
            {
                return NotFound();
            }
        }

    }
}
