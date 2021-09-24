using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PIVisionAttributeIdentifierUtility
{
   
    class SQLdata : DataTable
    {       
        public DataTable PullVisionAttributesGUIDlist(string sqlserver)
        {
            DataTable dataTable = new DataTable();
            string connString = $@"Server={sqlserver};Database=PIVision;Integrated Security=true;MultipleActiveResultSets=true"; /* ---> using integrated security*/
            string query =string.Format("SELECT a.[DisplayID],Name ,[Server] ,SUBSTRING(TRIM(SUBSTRING(Datasource,1, CHARINDEX('?',Datasource)-1) from Datasource),2,36)as Element_GUID,RIGHT(Datasource, 36) as Att_GUID FROM [PIVision].[dbo].[DisplayDatasources]a, [PIVision].[dbo].[View_DisplayList]b where a.DisplayID=b.DisplayID and Datasource like '%?%'");

            StringBuilder errorMessages = new StringBuilder();
        
                SqlConnection connection = new SqlConnection(connString);

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable); 
                connection.Close();
                adapter.Dispose();
         
            return dataTable;
        }

        public void TestingSQLConnection(string sqlserver)
        {
            string connString = $@"Server={sqlserver};Database=PIVision;Integrated Security=true;MultipleActiveResultSets=true"; /* ---> using integrated security*/
            SqlConnection connection = new SqlConnection(connString);  
            connection.Open();
        }

        public string ValidatingSQLConnection()
        {
            Utilities util = new Utilities();
            util.WriteInGreen("Enter the SQL Database instance hosting the PIVision database:");
            bool repeat = true;
            string sqlInstance = "";


            while (repeat)
            {
                Console.ForegroundColor = ConsoleColor.White;
                sqlInstance = Console.ReadLine();

                try
                {
                    TestingSQLConnection(sqlInstance);
                    repeat = false;
                }
                catch (SqlException ex)
                {
                    StringBuilder errorMessages = new StringBuilder();
                    util.WriteInRed("Could not connect to your PI Vision SQL database.");
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    util.WriteInRed(errorMessages.ToString());
                    util.WriteInGreen("Wrong. Enter the SQL Database instance hosting the PIVision database:");
                    repeat = true;
                }
            }
            return sqlInstance;
        }
    }
}
