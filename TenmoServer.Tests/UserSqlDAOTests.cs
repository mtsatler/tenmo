using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Tests
{
    [TestClass]
    public class UserSqlDAOTests : ServerBaseTest
    {

        [TestMethod]
        public void GetAllUsers()
        {

            UserSqlDAO dao = new UserSqlDAO(connectionString);

            List<User> users = dao.GetUsers();

            Assert.IsTrue(users.Count > 0);

        }



    }
}
