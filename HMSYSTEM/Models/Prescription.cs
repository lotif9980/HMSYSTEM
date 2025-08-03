
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.Models
{
    public class Prescription
    {
        public int Id { get; set; }             
        public DateTime? Date { get; set; }     
        public int PatientId { get; set; }       
        public int DoctorId { get; set; }        
        public int DepartmentId { get; set; }    
        public int? Status { get; set; }   
        public string ? Note { get; set; }
        public DateTime? NextFlowUp { get;set;}

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate {  get; set; }


        public Doctor? Doctor { get; set; } 
        public Patient? Patient { get; set; }
        public Department? Department { get; set; }

        //public List<PrescriptionDetail> PrescriptionDetails { get; internal set; }
        public List<PrescriptionDetail> PrescriptionDetails { get; set; } = new();
    }
}
