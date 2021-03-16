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
    public class UsersController : ControllerBase
    {

        private static IUserDAO userDao;

        public UsersController(IUserDAO _userDAO)
        {
            userDao = _userDAO;
        }


        [HttpGet]
        public List<User> ListUsers()
        {
            return userDao.GetUsers();
        }

        [HttpGet("{userName}")]
        public ActionResult<User> GetUser(string userName)
        {
            User user = userDao.GetUser(userName);

            if (user != null)
            {
                return user;
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("id/{userId}")]
        public ActionResult<User> GetUserById(int userId)
        {
            User user = userDao.GetUserById(userId);

            if(user != null)
            {
                return user;
            }
            else
            {
                return NotFound();
            }
        }


        /*not implemented yet

        [HttpGet("{userId}/accounts")]
        public List<Account> ListUserAccounts(int userId)
        {
            //return userDao.Accounts()
        }

        */

        /* not sure if route is needed below

        [HttpGet("{userId}/accounts/{accountId}")]
        public ActionResult<UsersController> GetUserAccount()
        {
            //Account account = accountDao.Get()
        }

        */



    }


}
