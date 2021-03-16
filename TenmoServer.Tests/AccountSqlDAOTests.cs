using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TenmoServer.Tests
{

    [TestClass]
    public class AccountSqlDAOTests : ServerBaseTest
    {
        [TestMethod]
        public void GetAccountTest()
        {

            AccountSqlDAO dao = new AccountSqlDAO(connectionString);

            Account account = dao.GetAccount(testUserOne.UserId);

            Assert.IsTrue(account.Balance == 1000.00M);

        }

        [TestMethod]
        public void GetAccountsTest()
        {
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);

            List<Account> accounts = dao.GetAccounts();

            Assert.IsTrue(accounts.Count > 0);

        }



    }
}
