using System;

namespace MIS_System_App.Models
{
    public class Account
    {
        public int Account_ID { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountCode { get; set; }
        public int? ParentAccountId { get; set; }
        public int Org_ID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string FullAccountCode => $"{AccountCode} - {AccountName}";
    }
}