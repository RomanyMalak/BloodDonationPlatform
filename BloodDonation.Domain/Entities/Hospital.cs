namespace BloodDonation.Domain.Entities;

public enum HospitalStatus
{
    Waiting = 0,
    Active = 1,
    Rejected = 2
}

public class Hospital
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // 3. التواصل والتوثيق الرسمي
    public string Hotline { get; set; } = string.Empty;
    public string LicenseDocumentPath { get; set; } = string.Empty; // مسار ملف الترخيص المرفوع

    // 4. التحكم في الصلاحية والمتابعة
    public bool IsActive { get; set; } = false; // لا تفعل إلا بموافقة الأدمن
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 5. العلاقات (Relationships)
    // علاقة (1 to Many) -> المستشفى الواحدة ترتبط بالعديد من طلبات الدم المحجوزة فيها
    public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
}
