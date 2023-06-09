﻿using Microsoft.Data.SqlClient;
using System.Data;

namespace CarGalleryAPI.Data
{
    public static class DbCreator
    {
        private static SqlConnection connection = new SqlConnection("Server=localhost;Database=master;User Id=sa;Password=123;TrustServerCertificate=true;");
        
        private static string dbExistSql = @"SELECT CASE WHEN DB_ID('CarGalleryDB') IS NULL THEN 0 ELSE 1 END";

        public static bool DoesDbExist()
        {
            SqlCommand sqlCommand = new SqlCommand(dbExistSql, connection);
            
            connection.Open();

            int result = (int)sqlCommand.ExecuteScalar();

            connection.Close();

            if (result == 0)
                return false;
            else
                return true;
        }

        public static void CreateDatabase()
        {
            string sqlQuery = File.ReadAllText("initial.sql");

            SqlCommand sqlCommand = new SqlCommand("CREATE DATABASE CarGalleryDB;", connection);

            connection.Open();

            sqlCommand.ExecuteNonQuery();

            sqlCommand = new SqlCommand(sqlQuery, connection);
            sqlCommand.ExecuteNonQuery();

            AddDataFromExcel();

            connection.Close();
        }

        private static void AddDataFromExcel()
        {
            DataSet ds = ExcelReader.ImportFromExcel();
            foreach (DataTable table in ds.Tables)
            {
                string sql = "";
                sql += $"INSERT INTO {table.TableName} (";
                int counter = 0;
                foreach (DataRow row in table.Rows)
                {
                    switch (counter){
                        case 0:
                            sql += $"{row["Column0"]}, {row["Column1"]}) VALUES ";
                            break;
                        case 1:
                            sql += $"('{row["Column0"]}', '{row["Column1"]}')";
                            break;
                        default:
                            sql += $",('{row["Column0"]}', '{row["Column1"]}')";
                            break;
                    }
                    counter++;
                }
                SqlCommand sqlCommand = new SqlCommand(sql, connection);
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
