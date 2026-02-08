using System.Collections.Generic;
using System.Data;
using Pharmacy_Management_AspNet_Webform.DAL;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform.BLL
{
    public class MedicineBLL
    {
        private MedicineDAL medicineDAL = new MedicineDAL();

        public List<Medicine> GetAllMedicines()
        {
            return medicineDAL.GetAllMedicines();
        }

        public Medicine GetMedicineById(int medicineId)
        {
            return medicineDAL.GetMedicineById(medicineId);
        }

        public void InsertMedicine(Medicine med)
        {
            medicineDAL.InsertMedicine(med);
        }

        public void UpdateMedicine(Medicine med)
        {
            medicineDAL.UpdateMedicine(med);
        }

        public void DeleteMedicine(int medicineId)
        {
            medicineDAL.DeleteMedicine(medicineId);
        }

        public DataTable GetMedicinesForDropdown()
        {
            return medicineDAL.GetMedicinesForDropdown();
        }

        public int CheckMedicineStock(int medicineId)
        {
            return medicineDAL.CheckMedicineStock(medicineId);
        }
    }
}
