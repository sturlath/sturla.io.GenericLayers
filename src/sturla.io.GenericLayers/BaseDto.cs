using System.Runtime.Serialization;

namespace sturla.io.GenericLayers
{
	public class BaseDto : IBaseDto
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }
    }
}