using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoClient.Data
{
    public class API_Transfer
    {
        public int transfer_id { get; set; }

        public int transfer_type_id { get; set; }

        public int transfer_status_id { get; set; }

        public int account_from { get; set; }

        public int account_to { get; set; }

        public decimal amount { get; set; }

        public API_Transfer()
        {
        }

        public API_Transfer(int transfer_type_id, int transfer_status_id, int account_from,
                              int account_to, decimal amount)
        {
            this.transfer_type_id = transfer_type_id;
            this.transfer_status_id = transfer_status_id;
            this.account_from = account_from;
            this.account_to = account_to;
            this.amount = amount;
        }
    }

    public class API_TransferStatus
    {
        public int transfer_status_id { get; set; }
        public string transfer_status_desc { get; set; }
    }

    public class API_TransferType
    {
        public int transfer_type_id { get; set; }
        public string transfer_type_desc { get; set; }
    }

}
