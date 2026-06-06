namespace BloodDonation.Domain.Entities;

public class Hospital
{
    // 1. المعرفات الأساسية والربط مع الـ Auth
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } // علاقة (1 to 1) مع جدول المستخدمين الأساسي لزوم الـ Login

    // 2. البيانات التعريفية والموقع
    public string Name { get; set; } = string.Empty;
    public string Government { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string AddressDetail { get; set; } = string.Empty;

    // الإحداثيات الجغرافية لمعالجة الـ Geo-Matching Agent
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
