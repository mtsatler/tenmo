using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TenmoServer.Tests
{
    [TestClass]
    public class TransferSqlDAOTests : ServerBaseTest
    {
        //test is broken not the DAO method
        
        [TestMethod]
        public void CreateNewTransferTest()
        {
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            Transfer transfer = dao.CreateTransfer(1, 1, testUserOne.UserId, testUserTwo.UserId, 100.00M);

            Assert.IsNotNull(transfer);

        }

        [TestMethod]
        public void ProcessTransferTest()
        {
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            AccountSqlDAO accDao = new AccountSqlDAO(connectionString);
            
            Transfer transfer = dao.CreateTransfer(2, 2, testUserOne.UserId, testUserTwo.UserId, 100.00M);

            Account userOneAccount = accDao.GetAccount(testUserOne.UserId);
            Account userTwoAccount = accDao.GetAccount(testUserTwo.UserId);

            Assert.AreEqual(900.00M, userOneAccount.Balance);
            Assert.AreEqual(1100.00M, userTwoAccount.Balance);

        }
        
    }
}
