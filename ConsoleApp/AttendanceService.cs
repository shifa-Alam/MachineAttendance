using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Newtonsoft.Json;

namespace ConsoleApp
{
    public class AttendanceService
    {
        public static string configFile = "";
        public static string folderPath = "";
        public static string baseUrl = "";
        private static Attendance attendance = new Attendance();

        public AttendanceService()
        {

        }

        public void FileWatch()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var parentDirectory = Directory.GetParent(Assembly.GetEntryAssembly().Location).Parent;
            configFile = parentDirectory.ToString() + "\\" + "config.txt";
            if (File.Exists(configFile))
            {
                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(configFile);
                folderPath = $"{lines[4]}";
                baseUrl = lines[5];
            }

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = folderPath;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {

        }
        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {

            try
            {
                ApplyConfigFileBl();
                var jsonData = ReadFileWithXlReaderBl(e.FullPath);
                //var jsonData = ReadFileWithExcelMapper(e.FullPath);
                _ = CallWebAPIAsync(jsonData);

                Console.WriteLine("File Created :{0}", e.Name);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void ApplyConfigFileBl()
        {

            Console.WriteLine(File.Exists(configFile) ? "Running App" : "Directory Not Found!");
            if (File.Exists(configFile))
            {
                string[] lines = File.ReadAllLines(configFile);
                attendance.HrConfigId = Convert.ToInt32(lines[0]);
                attendance.BranchId = Convert.ToInt32(lines[1]);
                attendance.StartDate = Convert.ToDateTime(lines[2]);
                attendance.EndDate = Convert.ToDateTime(lines[3]);

            }
        }

        private static string ReadFileWithXlReaderBl(string path)
        {
            try
            {
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);

                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //5. Data Reader methods
                excelReader.Read();
                while (excelReader.Read())
                {
                    var employeeAttendance = new EmployeeAttendance()
                    {
                        EmployeeId = Convert.ToInt64(excelReader.GetString(0)),
                        StartDate = (excelReader.GetString(4) == "C/In") ? Convert.ToDateTime(excelReader.GetString(3)) : null,
                        EndDate = (excelReader.GetString(4) == "C/Out") ? Convert.ToDateTime(excelReader.GetString(3)) : null,

                    };

                    attendance.EmployeeAttendances.Add(employeeAttendance);

                }

                //6. Free resources (IExcelDataReader is IDisposable)
                excelReader.Close();
                var json = JsonConvert.SerializeObject(attendance);
                Console.WriteLine(json);
                return json;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static async Task CallWebAPIAsync(string jsonData)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.PostAsync("api/HrAttendanceTest/SaveAttendanceAsync", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                if (response != null)
                {
                    Console.WriteLine(response.ToString());
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }


    }
}
