using System;

namespace MIS_System_App.Models
{
    public class User
    {
        public int User_ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string EncryptedPassword { get; set; }
        public int Org_ID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RoleName { get; set; }
        public int Role_ID { get; set; }
    }
}