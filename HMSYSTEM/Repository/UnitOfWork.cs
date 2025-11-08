
using HMSYSTEM.Data;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly Db db;
        IWebHostEnvironment env;

        public IPatientRepository PatienRepo { get; set; }
        public IDepartmentRepository departmentRepo { get; set; }
        public IDoctorRepository doctorRepo { get; set; }
        public IDesignationRepository designationRepo { get; set; }
        public IScheduleRepository scheduleRepo { get; set; }
        public IAppointmentRepository AppointmentRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IRoleRepository RoleRepository { get; set; }
        public IPatientHistoryRepository PatienHistoryRepo { get; set; }
        public IPrescriptionRepository PrescriptioRepository { get; set; }
        public IMedicineRepository MedicineRepo { get; set; }
        public IWardRepository wardRepository { get; set; }
        public IBedRepository bedRepository { get; set; }
        public IAdmissionRepository admissionRepository { get; set; }
        public IHomeRepository homeRepository { get; set; }
        public IServiceItemRepository serviceItemRepository {  get; set; }
        public IBillRepository billRepository { get; set; }
        public IReportRepository reportRepository {  get; set; }

        public UnitOfWork( Db _db, IWebHostEnvironment _env,
                          IPatientRepository _patientRepository,
                          IDepartmentRepository _departmentRepository,
                          IDoctorRepository _doctorRepository,
                          IDesignationRepository _designationRepository,
                          IScheduleRepository _scheduleRepository,
                          IAppointmentRepository _appointmentRepository,
                          IUserRepository _userRepository,
                          IRoleRepository _roleRepository,
                          IPatientHistoryRepository _patientHistoryRepository,
                          IPrescriptionRepository _prescriptionRepository,
                          IMedicineRepository _medicineRepository,
                          IWardRepository _wardRepository,
                          IBedRepository _bedRepository,
                          IAdmissionRepository _admissionRepository,
                          IHomeRepository _homeRepository,
                          IServiceItemRepository _serviceItemRepository,
                          IBillRepository _billRepository,
                          IReportRepository _reportRepository
                          )
        {
            this.env = _env;
            this.db = _db;
            PatienRepo = _patientRepository;
            departmentRepo= _departmentRepository;
            doctorRepo = _doctorRepository;
            designationRepo=_designationRepository;
            scheduleRepo=_scheduleRepository;
            AppointmentRepository=_appointmentRepository;
            UserRepository= _userRepository;
            RoleRepository= _roleRepository;
            PatienHistoryRepo= _patientHistoryRepository;
            PrescriptioRepository= _prescriptionRepository;
            MedicineRepo=_medicineRepository;
            wardRepository= _wardRepository;
            bedRepository= _bedRepository;
            admissionRepository=_admissionRepository;
            homeRepository= _homeRepository;
            serviceItemRepository = _serviceItemRepository;
            billRepository=_billRepository;
            reportRepository=_reportRepository;
        }

        public async Task<int> Save()
        {
            return await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void Complete()
        {
            db.SaveChanges();
        }

    }
}
