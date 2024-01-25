using System.ComponentModel.DataAnnotations;

namespace backend_pastebook_capstone.Models
{
	public class Photo
	{
		public Guid Id { get; set; }
		public string PhotoImageURL { get; set; } = null!;
		public DateTime UploadDate { get; set; } = DateTime.Now;

		public Guid AlbumId { get; set; }
		public Album? Album { get; set; }
	}

	public class PhotoDTO
	{
		public Guid Id { get; set; }
        public string PhotoImageURL { get; set; } = null!;
		public DateTime UploadDate { get; set; } = DateTime.Now;
		public Guid AlbumId { get; set; }
	}


}
