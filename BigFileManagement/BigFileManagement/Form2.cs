using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace BigFileManagement
{
    public partial class Form2 : Form
    {
        public DataTable sourceDataTable = new DataTable();

        public Form2()
        {
            InitializeComponent();
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split('|');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split('|');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            sourceDataTable = ConvertCSVtoDataTable(@"D:\CSVs\customer.csv");
            dataGridView1.DataSource = sourceDataTable;
        }

        private void btnSaveDB_Click(object sender, EventArgs e)
        {
            // take note of SqlBulkCopyOptions.KeepIdentity , you may or may not want to use this for your situation.  
            using (var bulkCopy = new SqlBulkCopy("Data Source=.;Initial Catalog=BMS_Maxin;Integrated Security=True;", SqlBulkCopyOptions.KeepIdentity))
            {
                // my DataTable column names match my SQL Column names, so I simply made this loop. However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
                foreach (DataColumn col in sourceDataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }

                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = "customer";
                bulkCopy.WriteToServer(sourceDataTable);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string s = "";
            int i = 0;
            float f = 0;
            decimal de = 0;
            char c = 's';
            double d = 0;

            Type sType = s.GetType();
            Type iType = i.GetType();
            Type fType = f.GetType();
            Type deType = de.GetType();
            Type cType = c.GetType();
            Type dType = d.GetType();

        }
    }
}
