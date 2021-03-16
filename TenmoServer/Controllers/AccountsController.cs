using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IAccountDAO accountDAO;
        public AccountsController(IAccountDAO _accountDAO)
        {
            accountDAO = _accountDAO;
        }
        [HttpGet]
        public ActionResult<List<Account>> ListAccounts()
        {
            return Ok(accountDAO.GetAccounts());
        }
        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(int id)
        {
            Account account = accountDAO.GetAccount(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpGet("user/{userId}")]
        
        public ActionResult<Account> GetUserAccounts(int userId)
        {
            List<Account> accounts = accountDAO.GetUserAccounts(userId);
            if(accounts == null)
            {
                return NotFound();
            }
            return Ok(accounts);
        }

       
        
        
        /*
        [HttpGet("{id}")]
        public ActionResult<decimal> GetBalance(int id)
        {
            Account account = accountDAO.GetAccount(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account.Balance);
        }
        */

       

        [HttpPost]
        public ActionResult<Account> AddAccount(int accountId, int userId, decimal balance)
        {
            //Account account = accountDAO.AddAccount(accountId, userId, balance);
            return Ok(accountDAO.AddAccount(accountId, userId, balance));
        }
    }
}
