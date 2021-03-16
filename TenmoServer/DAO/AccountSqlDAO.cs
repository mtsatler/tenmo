using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private readonly string connectionString;
        public AccountSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public Account GetAccount(int accountId)
        {
            Account returnAccount = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE account_id = @account_id", conn);
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnAccount = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnAccount;
        }
        public Account AddAccount(int accountId, int userId, decimal balance)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO accounts (account_id, user_id, balance) VALUES (@account_id, @user_id, @balance)", conn);
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@balance", balance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return GetAccount(accountId);
        }
        public List<Account> GetAccounts()
        {
            List<Account> returnAccounts = new List<Account>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Account u = GetAccountFromReader(reader);
                            returnAccounts.Add(u);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnAccounts;
        }

        public List<Account> GetUserAccounts(int userId)
        {
            List<Account> returnAccounts = new List<Account>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE user_id = @userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Account u = GetAccountFromReader(reader);
                            returnAccounts.Add(u);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnAccounts;


        }
        //public decimal GetBalance(int accountId)
        //{
        //    Account returnAccount = null;
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE account_id = @account_id", conn);
        //            cmd.Parameters.AddWithValue("@account_id", accountId);
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            if (reader.HasRows && reader.Read())
        //            {
        //                returnAccount = GetAccountFromReader(reader);
        //            }
        //        }
        //    }
        //    catch (SqlException)
        //    {
        //        throw;
        //    }
        //    return returnAccount.Balance;
        //}
        private Account GetAccountFromReader(SqlDataReader reader)
        {
            Account u = new Account()
            {
                AccountId = Convert.ToInt32(reader["account_id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                Balance = Convert.ToInt32(reader["balance"]),
            };

            return u;
        }
    }
}
