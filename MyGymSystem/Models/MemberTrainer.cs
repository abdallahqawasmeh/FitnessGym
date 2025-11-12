namespace MyGymSystem.Models
{
	public class MemberTrainer
	{
		public long MemberTrainerId { get; set; }
		public long MemberId { get; set; }
		public long TrainerId { get; set; }

		public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UnassignedAt { get; set; }
		public bool IsActive { get; set; } = true;

		public virtual Member Member { get; set; } = null!;
		public virtual Trainer Trainer { get; set; } = null!;
	}
}
