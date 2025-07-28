namespace POC.Models
    {
        public class Employee
        {
            public int? empId { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string designation { get; set; }
            public string ReportingTo { get; set; }
            public bool billable { get; set; }
            public string skill { get; set; }
            public string project { get; set; }
            public string location { get; set; }

            public DateOnly doj { get; set; }

            public string remarks { get; set; }

            
        }
    }

