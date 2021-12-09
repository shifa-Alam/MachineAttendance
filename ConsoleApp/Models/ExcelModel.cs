using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganss.Excel;

namespace ConsoleApp
{
    public class ExcelModel
    {
        [Column(1)]
        public string AcNo { get; set; }
        [Column(2)]
        public string No { get; set; }
        [Column(3)]
        public string Name { get; set; }
        [Column(4)]
        public string Time { get; set; }
        [Column(5)]
        public string State { get; set; }
        [Column(6)]
        public string NewState { get; set; }
        [Column(7)]
        public string Exception { get; set; }
        [Column(8)]
        public string Operation { get; set; }
    }
}
