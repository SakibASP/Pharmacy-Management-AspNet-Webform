using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Pharmacy_Management_AspNet_Webform.Common;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform.DAL
{
    internal sealed class SalesDAL
    {
        public string GetNextInvoiceNumber()
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.GetNextInvoiceNumber, con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        public int InsertSale(SalesMaster sale)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    SqlCommand cmdMaster = new SqlCommand(SpConstant.InsertSalesMaster, con, transaction);
                    cmdMaster.CommandType = CommandType.StoredProcedure;
                    cmdMaster.Parameters.AddWithValue("@InvoiceNumber", sale.InvoiceNumber);
                    cmdMaster.Parameters.AddWithValue("@InvoiceDate", sale.InvoiceDate);
                    cmdMaster.Parameters.AddWithValue("@CustomerName", sale.CustomerName);
                    cmdMaster.Parameters.AddWithValue("@CustomerContact", (object)sale.CustomerContact ?? DBNull.Value);
                    cmdMaster.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
                    cmdMaster.Parameters.AddWithValue("@Discount", sale.Discount);
                    cmdMaster.Parameters.AddWithValue("@GrandTotal", sale.GrandTotal);
                    SqlParameter outputParam = new SqlParameter("@Id", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    cmdMaster.Parameters.Add(outputParam);
                    cmdMaster.ExecuteNonQuery();
                    int invoiceId = (int)outputParam.Value;

                    foreach (SalesDetail detail in sale.Details)
                    {
                        SqlCommand cmdDetail = new SqlCommand(SpConstant.InsertSalesDetail, con, transaction);
                        cmdDetail.CommandType = CommandType.StoredProcedure;
                        cmdDetail.Parameters.AddWithValue("@InvoiceId", invoiceId);
                        cmdDetail.Parameters.AddWithValue("@MedicineId", detail.MedicineId);
                        cmdDetail.Parameters.AddWithValue("@BatchNo", (object)detail.BatchNo ?? DBNull.Value);
                        cmdDetail.Parameters.AddWithValue("@ExpiryDate", detail.ExpiryDate);
                        cmdDetail.Parameters.AddWithValue("@Quantity", detail.Quantity);
                        cmdDetail.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                        cmdDetail.Parameters.AddWithValue("@LineTotal", detail.LineTotal);
                        cmdDetail.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return invoiceId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<SalesMaster> GetAllSales()
        {
            List<SalesMaster> list = new List<SalesMaster>();
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.GetAllSales, con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SalesMaster
                    {
                        Id = (int)reader["Id"],
                        InvoiceNumber = reader["InvoiceNumber"].ToString(),
                        InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        CustomerContact = reader["CustomerContact"].ToString(),
                        SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                        Discount = Convert.ToDecimal(reader["Discount"]),
                        GrandTotal = Convert.ToDecimal(reader["GrandTotal"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                    });
                }
            }
            return list;
        }

        public SalesMaster GetSaleById(int invoiceId)
        {
            SalesMaster sale = null;
            using (SqlConnection con = DBHelper.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(SpConstant.GetSaleById, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", invoiceId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    sale = new SalesMaster
                    {
                        Id = (int)reader["Id"],
                        InvoiceNumber = reader["InvoiceNumber"].ToString(),
                        InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        CustomerContact = reader["CustomerContact"].ToString(),
                        SubTotal = Convert.ToDecimal(reader["SubTotal"]),
                        Discount = Convert.ToDecimal(reader["Discount"]),
                        GrandTotal = Convert.ToDecimal(reader["GrandTotal"]),
                        Details = new List<SalesDetail>()
                    };
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        sale.Details.Add(new SalesDetail
                        {
                            Id = (int)reader["Id"],
                            MedicineId = (int)reader["MedicineId"],
                            MedicineName = reader["MedicineName"].ToString(),
                            BatchNo = reader["BatchNo"].ToString(),
                            ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]),
                            Quantity = (int)reader["Quantity"],
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                            LineTotal = Convert.ToDecimal(reader["LineTotal"])
                        });
                    }
                }
            }
            return sale;
        }
    }
}
