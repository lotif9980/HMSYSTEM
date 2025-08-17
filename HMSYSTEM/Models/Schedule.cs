using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        [Required(ErrorMessage ="Doctor")]
        public int? DoctorId { get; set; }

        [Required(ErrorMessage = "Department")]
        public int ? DepartmentId { get; set; }

        [Required(ErrorMessage = "DayOfWeek")]
        public string ? DayOfWeek { get; set; }

        [Required(ErrorMessage = "StartTime")]
        public TimeSpan? StartTime { get; set; }

        [Required(ErrorMessage = "EndTime")]
        public TimeSpan? EndTime { get; set; }
        public bool? IsAvailable { get; set; } = true;

        public Department? Department { get; set; }
        public Doctor? Doctor { get; set; }

        public DateTime? Date { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateDate { get; set; }

    }

}
