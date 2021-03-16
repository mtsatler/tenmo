using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        Account GetAccount(int accountId);
        Account AddAccount(int accountId, int userId, decimal balance);
        List<Account> GetAccounts();
        List<Account> GetUserAccounts(int userId);
        
    }
}
