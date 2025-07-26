namespace HMSYSTEM.Enum
{
    
        public enum AppointmentStatus
        {
            Active = 1,
            InProgress = 2,
            Completed = 3,
            Deleted = 9
        }

        public enum WardType
        {
            General=1,
            Cabin=2,
            ICU=3,
            CCU=4,
            NICU=5,
            PICU=6,
            Emergency=7,
            Maternity=8,
            Isolation=9,
            Surgical=10
          
        }
        
        public enum BedType
        {
            General=1,
            SemiPrivate=2,
            Private=3,
            Deluxe=4,
            ICU=5,
            Burn=6
        }


}
