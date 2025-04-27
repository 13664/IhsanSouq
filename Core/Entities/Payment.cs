using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{

    public class Payment : BaseEntity
    {
        public string Uuid { get; set; }
        public int CharityCaseId { get; set; }
        public CharityCase CharityCase { get; set; }

        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime PaymentTime { get; set; }
        public string CardPan { get; set; }
    }
}
