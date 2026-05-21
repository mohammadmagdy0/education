using System;

namespace Education.API.Models
{
    public class ScratchCard
    {
        public int CardID { get; set; }
        
        // الكود السري الفريد المطبوع على الكارت (مثل: MATH-X9R2)
        public string SecureCode { get; set; } = string.Empty;
        
        // قيمة الكارت المالية (مثلاً 80 أو 100 جنيه)
        public decimal Value { get; set; }
        
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }

        // ربط الكارت بالطالب (Foreign Key)
        public int? UsedByStudentID { get; set; }
        public Student? UsedByStudent { get; set; }
    }
}