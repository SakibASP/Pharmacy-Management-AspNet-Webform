using Pharmacy_Management_AspNet_Webform.Common;
using Pharmacy_Management_AspNet_Webform.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Pharmacy_Management_AspNet_Webform.DAL
{
    internal sealed class MedicineDAL
    {
        public List<Medicine> GetAllMedicines()
        {
            List<Medicine> list = new List<Medicine>();
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 1); // operationId 1 for fetching all medicines
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Medicine
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        GenericName = reader["GenericName"].ToString(),
                        Category = reader["Category"].ToString(),
                        BatchNo = reader["BatchNo"].ToString(),
                        ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]),
                        Quantity = (int)reader["Quantity"],
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"])
                    });
                }
            }
            return list;
        }

        public Medicine GetMedicineById(int medicineId)
        {
            Medicine med = null;
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 2); // operationId 2 for fetching medicine by Id
                cmd.Parameters.AddWithValue("@Id", medicineId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    med = new Medicine
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        GenericName = reader["GenericName"].ToString(),
                        Category = reader["Category"].ToString(),
                        BatchNo = reader["BatchNo"].ToString(),
                        ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]),
                        Quantity = (int)reader["Quantity"],
                        UnitPrice = Convert.ToDecimal(reader["UnitPrice"])
                    };
                }
            }
            return med;
        }

        public void InsertMedicine(Medicine med)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 3); // operationId 3 for inserting new medicine
                cmd.Parameters.AddWithValue("@Name", med.Name);
                cmd.Parameters.AddWithValue("@GenericName", (object)med.GenericName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", (object)med.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BatchNo", med.BatchNo);
                cmd.Parameters.AddWithValue("@ExpiryDate", med.ExpiryDate);
                cmd.Parameters.AddWithValue("@Quantity", med.Quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", med.UnitPrice);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMedicine(Medicine med)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 4); // operationId 4 for updating medicine
                cmd.Parameters.AddWithValue("@Id", med.Id);
                cmd.Parameters.AddWithValue("@Name", med.Name);
                cmd.Parameters.AddWithValue("@GenericName", (object)med.GenericName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Category", (object)med.Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BatchNo", med.BatchNo);
                cmd.Parameters.AddWithValue("@ExpiryDate", med.ExpiryDate);
                cmd.Parameters.AddWithValue("@Quantity", med.Quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", med.UnitPrice);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMedicine(int medicineId)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 5); // operationId 5 for deleting medicine
                cmd.Parameters.AddWithValue("@Id", medicineId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetMedicinesForDropdown()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 6); // operationId 6 for fetching medicines for dropdown
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int CheckMedicineStock(int medicineId)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.MedicineOperations, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OperationId", 7); // operationId 7 for medicine stock check
                cmd.Parameters.AddWithValue("@Id", medicineId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }
}
