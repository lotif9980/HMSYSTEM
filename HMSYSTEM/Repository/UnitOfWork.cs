



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




        public UnitOfWork( Db _db, IWebHostEnvironment _env)
        {
            this.env = _env;
            this.db = _db;

            PatienRepo = new PatientRepository(db,env);
            departmentRepo = new DepartmentRepository(db);
            doctorRepo = new DoctorRepository(db,env);
            designationRepo = new DesignationRepository(db);
            scheduleRepo = new ScheduleRepository(db);
            AppointmentRepository = new AppointmentRepository(db);
            UserRepository = new UserRepository(db);
            RoleRepository= new RoleRepository(db);
            PatienHistoryRepo = new PatientHistoryRepository(db);



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
