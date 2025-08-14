namespace POC.Models
{
    public class DashboardMetrics
    {

        public int TotalEmployees { set; get; }
        public int ProjectsActive { get; set; }
        public int Billable_FTEs { get; set; }
        public int UnassignedEmployees { get; set; }
    }

    // RoleCount.cs
    public class RoleCount
    {
        public string Role { get; set; }
        public int Count { get; set; }
    }

    // AssignedProjects.cs
    public class AssignedProjects
    {
        public string Project { get; set; }
        public int AssignedEmployees { get; set; }
    }

    // DashboardData.cs (A container to hold all three result sets)
    public class DashboardData
    {
        public DashboardMetrics Metrics { get; set; }
        public List<RoleCount> EmployeeDistribution { get; set; }
        public List<AssignedProjects> ProjectAssignments { get; set; }
    }

}
