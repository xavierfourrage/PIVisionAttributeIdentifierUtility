using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using OSIsoft.AF;

namespace PIVisionAttributeIdentifierUtility
{
   
    class SQLdata : DataTable
    {       
        public DataTable PullVisionAttributesGUIDlist(string sqlserver)
        {
            DataTable dataTable = new DataTable();
            string connString = $@"Server={sqlserver};Database=PIVision;Integrated Security=true;MultipleActiveResultSets=true"; /* ---> using integrated security*/
            /*            string query =string.Format("SELECT a.[DisplayID],Name ,[Server] ,SUBSTRING(TRIM(SUBSTRING(Datasource,1, CHARINDEX('?',Datasource)-1) from Datasource),2,36)as Element_GUID,RIGHT(Datasource, 36) as Att_GUID FROM [PIVision].[dbo].[DisplayDatasources]a, [PIVision].[dbo].[View_DisplayList]b where a.DisplayID=b.DisplayID and Datasource like '%?%'");
            */
            string query =string.Format(" SELECT a.[DisplayID],Name ,[Server] , FullDatasource  FROM [PIVision].[dbo].[DisplayDatasources]a, [PIVision].[dbo].[View_DisplayList]b where a.DisplayID=b.DisplayID  and FullDatasource like '%|%'");
                
                SqlConnection connection = new SqlConnection(connString);

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable); 
                connection.Close();
                adapter.Dispose();
         
            return dataTable;
        }

        public DataTable FormatDatable(DataTable datatable)
        {           
           datatable.Columns.Add("AFDatabase", typeof(string));
           datatable.Columns.Add("AttributePath", typeof(string));

            for (int i = 0; i < datatable.Rows.Count; i++)
            {
                string FullDataSource = datatable.Rows[i][3].ToString();
                if(FullDataSource.Contains("?"))
                {
                    
                  /*  FullDataSource = FullDataSource.TrimStart('\\');*/
                    string[] subs = FullDataSource.Split('?');
                    string[] subs1 = FullDataSource.Split('\\');
                    string databasename = subs1[3];
                    string elementPath = subs[0];
                    string [] subs2 = FullDataSource.Split('|');
                    string attributeName = subs2[1].Split('?')[0];
                    string attributePath = elementPath + "|"+attributeName;

                    datatable.Rows[i]["AFDatabase"] = databasename;
                    datatable.Rows[i]["AttributePath"] = attributePath;
                }
                else
                {                   
                    string[] subs1 = FullDataSource.Split('\\');
                    string databasename = subs1[3];
                    datatable.Rows[i]["AFDatabase"] = databasename;
                    datatable.Rows[i]["AttributePath"] = FullDataSource;
                }              
            }
            return datatable;

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
