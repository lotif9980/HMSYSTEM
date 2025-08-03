using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int? DoctorId { get; set; }
        public int ? DepartmentId { get; set; } 
        public string ? DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }
        public bool? IsAvailable { get; set; }

        public Department? Department { get; set; }
        public Doctor? Doctor { get; set; }

        public DateTime? Date { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDate { get; set; }

    }

}
