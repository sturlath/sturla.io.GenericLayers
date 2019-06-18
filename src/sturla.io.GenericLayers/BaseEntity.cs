using System;
using System.ComponentModel.DataAnnotations;

namespace sturla.io.GenericLayers
{
	public abstract class BaseEntity : IBaseEntity
	{
		public int Id { get; set; }

		[Timestamp]
		public byte[] Timestamp { get; set; }

		public DateTime CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string CreatedBy { get; set; }
		public string ModifiedBy { get; set; }
	}
}