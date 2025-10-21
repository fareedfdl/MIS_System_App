using System;

namespace MIS_System_App.Models
{
    public class Organization
    {
        public int Org_ID { get; set; }
        public string CompanyName { get; set; }
        public string BaseCurrency { get; set; }
        public string Timezone { get; set; }
        public string BaseLanguage { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string DisplayInfo => $"{CompanyName} - {BaseCurrency}";
    }
}