using System;
using System.Collections.Generic;

namespace Education.API.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // بصمة الجهاز: لمنع فتح الحساب من أكثر من جهاز
        public string? DeviceID { get; set; } 
        
        // المحفظة المالية للطالب لشراء الحصص
        public decimal WalletBalance { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // العلاقات (Navigation Properties)
        public ICollection<ScratchCard> UsedCards { get; set; } = new List<ScratchCard>();
    }
}