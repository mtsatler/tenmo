using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;
        private IAccountDAO accountDAO;
        public TransferSqlDAO(string dbConnectionString, IAccountDAO dao = null)
        {
            connectionString = dbConnectionString;
            if (dao == null)
            {
                accountDAO = new AccountSqlDAO(connectionString);
            }
            else
            {
                accountDAO = dao;
            }
        }
        
        public Transfer CreateTransfer(int transferTypeId, int transferStatusId, int fromAccount, int toAccount, decimal amount)
        {
            int transferId = 0;
            try
            {
                string statusText = "";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount)", conn);
                    cmd.Parameters.AddWithValue("@transfer_type_id", transferTypeId);
                    cmd.Parameters.AddWithValue("@transfer_status_id", transferStatusId);
                    cmd.Parameters.AddWithValue("@account_from", fromAccount);
                    cmd.Parameters.AddWithValue("@account_to", toAccount);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT max(transfer_id) FROM transfers", conn);
                    transferId = Convert.ToInt32(cmd.ExecuteScalar());

                    
                    
                    cmd = new SqlCommand("SELECT transfer_status_desc FROM transfer_statuses WHERE transfer_status_id = @transfer_status_id", conn);
                    cmd.Parameters.AddWithValue("@transfer_status_id", transferStatusId);
                    statusText = Convert.ToString(cmd.ExecuteScalar());

                    
                }
                try
                {
                    if (statusText.ToUpper() == "APPROVED")
                    {
                        if (!ProcessTransfer(fromAccount, toAccount, amount))
                        {
                            Transfer updateStatus = GetTransfer(transferId);
                            updateStatus.transfer_status_id = 3;
                            UpdateTransfer(updateStatus);
                        }
                            
                    }
                   
                }
                catch (Exception)
                {
              
          
                }

            }
            catch (SqlException)
            {            
                throw new Exception("SQL Error");
            }
            return GetTransfer(transferId);
        }

        public List<Transfer> GetTransfersByUser(int userId)
        {
            List<Transfer> returnTransfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers WHERE account_from = @account_id OR account_to = @account_id", conn);
                    cmd.Parameters.AddWithValue("@account_id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Transfer u = GetTransferFromReader(reader);
                            returnTransfers.Add(u);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnTransfers;
        }
        public Transfer GetTransfer(int id)
        {
            Transfer returnTransfer = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfers WHERE transfer_id = @transfer_id", conn);
                    cmd.Parameters.AddWithValue("@transfer_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnTransfer = GetTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnTransfer;
        }

        public TransferStatus GetTransferStatusById(int statusId)
        {
            TransferStatus returnStatus = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_status_id, transfer_status_desc FROM transfer_statuses WHERE transfer_status_id = @transferStatusId", conn);
                    cmd.Parameters.AddWithValue("@transferStatusId", statusId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnStatus = GetTransferStatusFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnStatus;

        }
        
        public TransferType GetTransferTypeById(int typeId)
        {
            TransferType returnType = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_type_id, transfer_type_desc FROM transfer_types WHERE transfer_type_id = @transferTypeId", conn);
                    cmd.Parameters.AddWithValue("@transferTypeId", typeId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnType = GetTransferTypeFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnType;
        }


        private bool ProcessTransfer(int accountFrom, int accountTo, decimal amount)
        {
            Account fromAcc = accountDAO.GetAccount(accountFrom);
            Account toAcc = accountDAO.GetAccount(accountTo);

            fromAcc.Balance -= amount;
            toAcc.Balance += amount;

            if(fromAcc.Balance >= amount)
            {

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {

                        conn.Open();

                        //will process transfer commands together as transaction, we need to make sure both work or none at all
                        SqlTransaction transaction;

                        transaction = conn.BeginTransaction("Process Transfer");

                        SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = @amount WHERE account_id = @from_id", conn);
                        cmd.Transaction = transaction;
                        cmd.Parameters.AddWithValue("@amount", fromAcc.Balance);
                        cmd.Parameters.AddWithValue("@from_id", fromAcc.AccountId);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "UPDATE accounts SET balance = @amountTo WHERE account_id = @to_id";
                        cmd.Parameters.AddWithValue("@amountTo", toAcc.Balance);
                        cmd.Parameters.AddWithValue("@to_id", toAcc.AccountId);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();

                        return true;

                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return false;
            }

        }

        public Transfer UpdateTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_status_id = @transfer_status_id WHERE transfer_id = @transfer_id", conn);
                cmd.Parameters.AddWithValue("@transfer_status_id", transfer.transfer_status_id);
                cmd.Parameters.AddWithValue("@transfer_id", transfer.transfer_id);
                cmd.ExecuteNonQuery();
            }
            if (transfer.transfer_status_id == 2)
            {
                ProcessTransfer(transfer.account_from, transfer.account_to, transfer.amount);
            }
            return GetTransfer(transfer.transfer_id);
        }
        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer u = new Transfer();

            u.transfer_id = Convert.ToInt32(reader["transfer_id"]);
            u.transfer_type_id = Convert.ToInt32(reader["transfer_type_id"]);
            u.transfer_status_id = Convert.ToInt32(reader["transfer_status_id"]);
            u.account_from = Convert.ToInt32(reader["account_from"]);
            u.account_to = Convert.ToInt32(reader["account_to"]);
            u.amount = Convert.ToDecimal(reader["amount"]);
            
            return u;
        }

        private TransferStatus GetTransferStatusFromReader(SqlDataReader reader)
        {
            TransferStatus u = new TransferStatus()
            {
                transfer_status_id = Convert.ToInt32(reader["transfer_status_id"]),
                transfer_status_desc = Convert.ToString(reader["transfer_status_desc"])
            };

            return u;
        }

        private TransferType GetTransferTypeFromReader(SqlDataReader reader)
        {
            TransferType u = new TransferType()
            {
                transfer_type_id = Convert.ToInt32(reader["transfer_type_id"]),
                transfer_type_desc = Convert.ToString(reader["transfer_type_desc"])
            };

            return u;
        }

    }
}
