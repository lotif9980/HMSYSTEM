


namespace HMSYSTEM.Repository
{
    public interface IUnitOfWork
    {
        IPatientRepository PatienRepo { get; }
        IDepartmentRepository departmentRepo { get; }
        IDoctorRepository doctorRepo { get; }
        IDesignationRepository designationRepo { get; }
        IScheduleRepository scheduleRepo { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        void Complete();
        Task<int> Save();
    }
}
