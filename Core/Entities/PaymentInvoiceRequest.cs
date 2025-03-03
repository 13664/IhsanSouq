using System;

namespace Core.Entities;

public class PaymentInvoiceRequest
{

        public int store_id { get; set; } // Your assigned store ID
        public int amount { get; set; }     // Invoice amount in tiyins (smallest currency unit)
        public string invoice_id { get; set; }  // Unique invoice ID from your system
        public string return_url { get; set; }  // (Optional) URL for user return after payment
        public string callback_url { get; set; } // (Optional) URL where the payment gateway will send the callback after payment
    
    }
