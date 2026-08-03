using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=crd-mssql-db,1433;Database=crd-mssql-db;User Id=sa;Password=Str0p@ssword;Encrypt=false;";
        
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("✅ Connected to database successfully!");
                Console.WriteLine("\n📊 Tables in database:");
                Console.WriteLine("=====================================");
                
                string query = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
                               WHERE TABLE_TYPE='BASE TABLE' 
                               ORDER BY TABLE_NAME";
                
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                
                int count = 0;
                while (reader.Read())
                {
                    string tableName = reader.GetString(0);
                    Console.WriteLine($"  • {tableName}");
                    count++;
                }
                
                Console.WriteLine("=====================================");
                Console.WriteLine($"Total: {count} tables");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }
}
