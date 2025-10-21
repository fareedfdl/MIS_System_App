using System;

namespace MIS_System_App.Models
{
    public class Transaction
    {
        public int Trans_ID { get; set; }
        public int DebitAccountId { get; set; }
        public int CreditAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string ReferenceNumber { get; set; }
        public int Org_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string DebitAccountName { get; set; }
        public string CreditAccountName { get; set; }
        public string DisplayInfo => $"{TransactionDate:yyyy-MM-dd} - {Amount:C0} - {Description}";
    }
}