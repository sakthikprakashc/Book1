using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BigFileManagement
{
    public partial class Form1 : Form
    {
        public DataTable empDataTable = new DataTable();
        public DataSet empDataSet = new DataSet();
        public string FolderGuid = System.Guid.NewGuid().ToString();

        public List<int> chunkSplitter = new List<int>() { 3, 3, 3, 1 };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            logic3();
        }

        public void logic3()
        {
            empDataTable = EmployeeDataTable();
        }

        public void logic2()
        {
            const int bufferSize = 1000000; //1 MB

            var sb = new StringBuilder();
            var buffer = new char[bufferSize];
            var length = 0L;
            var totalRead = 0L;
            var count = bufferSize;
            var counter = 0;

            var splitFileContent = new List<string>();

            using (var sr = new StreamReader(@"D:\6MB_BigTextFile.txt"))
            {
                length = sr.BaseStream.Length;
                while (count > 0)
                {
                    count = sr.Read(buffer, 0, bufferSize);
                    sb.Append(buffer, 0, count);

                    splitFileContent.Add(new string(buffer));

                    totalRead += count;
                    counter++;
                }
            }
            MessageBox.Show(totalRead.ToString());
        }

        public void Logic1()
        {
            string filename = "D:\\6MB_BigTextFile.txt";
            StringBuilder sb = new StringBuilder();
            foreach (int progress in LoadFileWithProgress(filename, sb))
            {
                // Update your progress counter here!
            }
            string fileData = sb.ToString();
        }

        public IEnumerable<int> LoadFileWithProgress(string filename, StringBuilder stringData)
        {
            const int charBufferSize = 4096;
            using (FileStream fs = File.OpenRead(filename))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    long length = fs.Length;
                    int numberOfChunks = Convert.ToInt32((length / charBufferSize)) + 1;
                    double iter = 100 / Convert.ToDouble(numberOfChunks);
                    double currentIter = 0;
                    yield return Convert.ToInt32(currentIter);
                    while (true)
                    {
                        char[] buffer = br.ReadChars(charBufferSize);
                        if (buffer.Length == 0) break;
                        stringData.Append(buffer);
                        currentIter += iter;
                        yield return Convert.ToInt32(currentIter);
                    }
                }
            }
        }

        private DataTable EmployeeDataTable()
        {
            DataTable employeeDataTable = new DataTable();

            employeeDataTable.Columns.Add("Employee Id");
            employeeDataTable.Columns.Add("Employee Number");
            employeeDataTable.Columns.Add("Employee Name");
            employeeDataTable.Columns.Add("Age");
            employeeDataTable.Columns.Add("Salary");

            employeeDataTable.Rows.Add(new object[] { "1", "EMP001", "Akash Nayak", "34", "15000" });
            employeeDataTable.Rows.Add(new object[] { "2", "EMP002", "Boominathan", "33", "48000" });
            employeeDataTable.Rows.Add(new object[] { "3", "EMP003", "Charles Stephen", "36", "54000" });
            employeeDataTable.Rows.Add(new object[] { "4", "EMP004", "Daniel Raj", "31", "42000" });
            employeeDataTable.Rows.Add(new object[] { "5", "EMP005", "Fazil Antony", "30", "36000" });
            employeeDataTable.Rows.Add(new object[] { "6", "EMP006", "Gowtham Shankar", "29", "41000" });
            employeeDataTable.Rows.Add(new object[] { "7", "EMP007", "Harish Kalyan", "24", "31000" });
            employeeDataTable.Rows.Add(new object[] { "8", "EMP008", "Julie Claral Mary", "28", "36000" });
            employeeDataTable.Rows.Add(new object[] { "9", "EMP009", "Kishore Kumar", "31", "29000" });
            employeeDataTable.Rows.Add(new object[] { "10", "EMP010", "Louis Kirshtopher", "38", "44000" });

            return employeeDataTable;
        }

        private DataTable GetChunkedDataTable(int startIndex, int length)
        {
            int endIndex = startIndex + length;
            DataTable chunkedDataTable = empDataTable.Clone();

            for (int i = startIndex; i < endIndex; i++)
            {
                chunkedDataTable.Rows.Add(empDataTable.Rows[i].ItemArray);
            }

            return chunkedDataTable;
        }

        public void BuildChunks(List<int> chunkSplitter)
        {
            int cCount = chunkSplitter.Count;
            DataTable[] _dataTableChunk = new DataTable[cCount];

            int startPosition = 0;

            for (int i = 0; i < cCount; i++)
            {
                _dataTableChunk[i] = new DataTable();
                if (i == 0)
                {
                    _dataTableChunk[i] = GetChunkedDataTable(0, chunkSplitter[i]);
                }
                else
                {
                    _dataTableChunk[i] = GetChunkedDataTable(startPosition, chunkSplitter[i]);
                }
                startPosition += chunkSplitter[i];

                _dataTableChunk[i].TableName = "Chunk_" + i;
                empDataSet.Tables.Add(_dataTableChunk[i]);
            }
        }

        private void ProcessChunks()
        {
            if (empDataSet.Tables.Count > 0)
            {
                foreach (DataTable dTable in empDataSet.Tables)
                {
                    dataGridView1.DataSource = empDataSet.Tables[0];
                    ToCSV(dTable, dTable.TableName);
                }
            }
        }

        public void ToCSV(DataTable dtDataTable, string fileName)
        {
            string tempPath = Path.GetTempPath();

            string folderPath = Path.Combine(tempPath, FolderGuid);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            StreamWriter sw = new StreamWriter(Path.Combine(folderPath, fileName + ".csv"), false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(","))
                        {
                            value = string.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        public void ConsolidataCSVs()
        {
            string[] filePaths = Directory.GetFiles(Path.Combine(Path.GetTempPath(), FolderGuid), "*.csv");
            StreamWriter fileDest = new StreamWriter(Path.Combine(Path.GetTempPath(), FolderGuid, "Consolidated.csv"), true);

            for (int i = 0; i < filePaths.Length; i++)
            {
                string file = filePaths[i];

                string[] lines = File.ReadAllLines(file);

                if (i > 0)
                {
                    lines = lines.Skip(1).ToArray(); // Skip header row for all but first file
                }

                foreach (string line in lines)
                {
                    fileDest.WriteLine(line);
                }

                if (File.Exists(filePaths[i]))
                    File.Delete(filePaths[i]);
            }

            fileDest.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BuildChunks(chunkSplitter);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcessChunks();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConsolidataCSVs();
        }
    }
}