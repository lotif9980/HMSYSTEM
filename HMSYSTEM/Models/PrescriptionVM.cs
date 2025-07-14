namespace HMSYSTEM.Models
{
    public class PrescriptionVM
    {
        // Prescription Main Info
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? DepartmentId { get; set; }
        public int? Status { get; set; }

        // Optional names for display
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? DepartmentName { get; set; }
        public List<PrescriptionDetail> Details { get; set; } = new();

    }
}
