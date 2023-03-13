using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UnityXUMMAPI.Models;
using UnityXUMMAPI.Repository;

namespace UnityXUMMAPI.Entities
{
	[Table("PayloadStatus")]
	public class PayloadStatus:BaseEntity
    {
		public bool Signed { get; set; }
		public string Account { get; set; }
		[Key]
		public string PayloadUUid { get; set; }
		public bool IsActive { get; set; }
		
		public DateTime UpdatedOn { get; set; }
    }
}
