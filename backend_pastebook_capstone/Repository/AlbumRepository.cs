using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;

namespace backend_pastebook_capstone.Repository
{
	public class AlbumRepository
	{
		private readonly CapstoneDBContext _context;

		public AlbumRepository(CapstoneDBContext context)
		{
			_context = context;
		}

		public Album? GetAlbumByAlbumId(Guid? albumId)
		{
			return _context.Album.FirstOrDefault(a => a.Id == albumId);
		}

		public List<Album> GetAlbumsListByUserId(Guid userId)
		{
			return _context.Album.Where(a => a.UserId == userId).ToList();
		}

		public List<Album> GetAlbumsLimitBySix(Guid userId)
		{
			return _context.Album.Where(a => a.UserId == userId).Take(6).ToList();
		}
		public List<Photo> GetPhotosByAlbumId(Guid albumId)
		{
			return _context.Photo.Where(p => p.AlbumId == albumId).ToList();
		}

		public Album? GetUploadsAlbumId(Guid userId)
		{
			return _context.Album.FirstOrDefault(a => a.AlbumName == "Uploads" && a.UserId == userId);
		}

		public void AddAlbum(Album album)
		{
			_context.Album.Add(album);
			_context.SaveChanges();
		}
		public void UpdateAlbum(Album album)
		{
			_context.Album.Update(album);
			_context.SaveChanges();
		}
		public void RemoveAlbum(Album album)
		{
			_context.Album.Remove(album);
			_context.SaveChanges();
		}
	}
}
