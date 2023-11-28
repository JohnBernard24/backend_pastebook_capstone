using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;

namespace backend_pastebook_capstone.Repository
{
	public class PhotoRepository
	{
		private readonly CapstoneDBContext _context;

		public PhotoRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public Photo? GetPhotoByPhotoId(Guid? photoId)
		{
			return _context.Photo.FirstOrDefault(p => p.Id == photoId);
		}

		public Photo? GetFirstPhotoForAlbum(Guid? albumId)
		{
			return _context.Photo
				.Where(photo => photo.AlbumId == albumId)
				.OrderBy(photo => photo.UploadDate)
				.FirstOrDefault();
		}

		public void AddPhoto(Photo photo)
		{
			_context.Photo.Add(photo);
			_context.SaveChanges();
		}

		public void UpdatePhoto(Photo photo)
		{
			_context.Photo.Update(photo);
			_context.SaveChanges();
		}
		public void RemovePhoto(Photo photo)
		{
			_context.Photo.Remove(photo);
			_context.SaveChanges();
		}
	}
}
