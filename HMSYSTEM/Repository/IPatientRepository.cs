using HMSYSTEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Repository
{
    public interface IPatientRepository
    {
        public List<Patient> getAll();
        public Patient Details(int Id);
        public List<Patient> Delete(int Id);
        public Patient Edit(int Id);
        public List<Patient> Save(Patient Patient);
        public Patient Update(Patient patient);
    }
}
