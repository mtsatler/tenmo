using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int transfer_id { get; set; }

        public int transfer_type_id { get; set; }

        public int transfer_status_id { get; set; }

        public int account_from { get; set; }

        public int account_to { get; set; }

        public decimal amount { get; set; }
    }

    public class TransferStatus
    {
        public int transfer_status_id { get; set; }
        public string transfer_status_desc { get; set; }
    }

    public class TransferType
    {
        public int transfer_type_id { get; set; }
        public string transfer_type_desc { get; set; }
    }

}
