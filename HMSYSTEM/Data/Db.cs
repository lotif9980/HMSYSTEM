using HMSYSTEM.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;

using static System.Net.Mime.MediaTypeNames;

namespace HMSYSTEM.Data
{
    public class Db:DbContext
    {
        public static string ConnectionString = "Server=localhost;Database=hmsystem;User Id=sa;Password=Test_123;Encrypt=False";

        //public static string ConnectionString = "Server=103.125.252.243;Database=cecom;User Id=oct_cecom;Password=ywvflaxd2hobusjcznmt;Encrypt=False";

        public Db()
        {
            
        }

        public Db(DbContextOptions<Db> options):base(options) 
        {
        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }

       

            public DbSet<Patient> Patients { get; set; }
            public DbSet<Department>Departments { get; set; }
            public DbSet<Doctor>Doctors { get; set; }
            public DbSet<Designation> Designations { get; set; }
            public DbSet<Schedule> Schedules { get; set; }
            public DbSet<Appointment> Appointments { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<Role> Roles { get; set; }
            public DbSet<PatientHistory> PatientHistorys { get; set; }
            public DbSet<Prescription> Prescriptions { get; set; }
            public DbSet<PrescriptionDetail>PrescriptionDetails { get; set; }
            public DbSet<Medicine> Medicines { get; set; }
            public DbSet<Ward> Wards { get; set; }


    }
}
