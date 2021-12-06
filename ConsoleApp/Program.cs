using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp
{
    class Program
    {
        private static string folderPath;
        private static Attendance attendance = new Attendance();
       private static  string textFile = @"C:/Users/shifa/Desktop/machineAttendanceConfigure.txt";

        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Console.WriteLine("Folder Path : ");
            folderPath = Console.ReadLine();
            folderPath = $"{folderPath}";


            //TakeInputBl(attendance);
            MonitorFolderBl();
            Console.ReadKey();

        }

        private static void MonitorFolderBl()
        {
            try
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
                fileSystemWatcher.Path = folderPath;
                fileSystemWatcher.Created += FileSystemWatcher_Created;
                fileSystemWatcher.EnableRaisingEvents = true;

            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {

            try
            {
              

                if (File.Exists(textFile))
                {
                    // Read a text file line by line.  
                    string[] lines = File.ReadAllLines(textFile);
                    attendance.HrConfigId = Convert.ToInt32(lines[0]);
                    attendance.BranchId = Convert.ToInt32(lines[1]);
                    attendance.StartDate = Convert.ToDateTime(lines[2]);
                    attendance.EndDate = Convert.ToDateTime(lines[3]);
                }


                var jsonData = ReadFileWithXlReaderBl(e.FullPath);

                _ = CallWebAPIAsync(jsonData);

                Console.WriteLine("File Created :{0}", e.Name);
            }
            catch (Exception exception)
            {
                    throw;
            }
        }
        private static string ReadFileWithXlReaderBl(string path)
        {
            try
            {
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);

                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                //...
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
                client.BaseAddress = new Uri("http://localhost:44317/");
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



        private static void TakeInputBl(Attendance entity)
        {

            try
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));

                Console.WriteLine("Hr Config Id : ");
                entity.HrConfigId =Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();

                Console.WriteLine("Branch Id :");
                entity.BranchId = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();

                Console.WriteLine("Start Time :");
                entity.StartDate = Convert.ToDateTime(Console.ReadLine());
                Console.WriteLine();

                Console.WriteLine("End Time :");
                entity.EndDate = Convert.ToDateTime(Console.ReadLine());
                Console.WriteLine();

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
