using System;
namespace UnityXUMMAPI.Models
{
	public class CheckStatus
	{
		public bool Signed { get; set; }
		public string Account { get; set; }
		public string PayloadUUid { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatonOn { get; set; } = DateTime.Now;
		public DateTime UpdatedOn { get; set; }

	}
}

