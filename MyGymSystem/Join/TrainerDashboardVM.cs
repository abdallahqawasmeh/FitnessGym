namespace MyGymSystem.Join
{
    public class TrainerDashboardVM
    {
        public int MyMembersCount { get; set; }
        public int TotalPlans { get; set; }
        public int ActivePlans { get; set; }
        public List<RecentPlanRow> RecentPlans { get; set; } = new();

        public class RecentPlanRow
        {
            public long Workoutplanid { get; set; }
            public long Memberid { get; set; }
            public string MemberName { get; set; } = "";
            public string Planname { get; set; } = "";
            public DateTime? Startdate { get; set; }
            public bool? Status { get; set; }
        }
    }

}
