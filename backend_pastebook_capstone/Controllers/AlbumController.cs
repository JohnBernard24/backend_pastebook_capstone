using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/album")]
	[ApiController]
	public class AlbumController : ControllerBase
	{
		private readonly UserRepository _userRepository;
		private readonly AlbumRepository _albumRepository;
		private readonly PhotoRepository _photoRepository;

		public AlbumController(UserRepository userRepository, AlbumRepository albumRepository, PhotoRepository photoRepository)
		{
			_userRepository = userRepository;
			_albumRepository = albumRepository;
			_photoRepository = photoRepository;
		}


		[HttpPost("add-album")]
		public IActionResult AddAlbum([FromBody] AlbumDTO albumDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_album" });

			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
				return BadRequest(new { result = "invalid_user_id" });

			Album album = new Album
			{
				AlbumName = albumDTO.AlbumName,
				UserId = user.Id,
				User = user
			};

			_albumRepository.AddAlbum(album);

			return Ok(album.Id);
		}

		[HttpPut("update-album")]
		public IActionResult UpdateAlbum([FromBody] AlbumDTO albumDTO)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			if (!ModelState.IsValid)
				return BadRequest(new { result = "invalid_album" });

			Album? existingAlbum = _albumRepository.GetAlbumByAlbumId(albumDTO.Id);

			if (existingAlbum == null)
				return NotFound(new { result = "album_not_found" });

			existingAlbum.AlbumName = albumDTO.AlbumName;

			_albumRepository.UpdateAlbum(existingAlbum);

			albumDTO.Id = existingAlbum.Id;
			albumDTO.UserId = existingAlbum.UserId;

			return Ok(albumDTO);
		}

		[HttpDelete("delete-album/{albumId}")]
		public IActionResult DeleteAlbum(Guid albumId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			Album? existingAlbum = _albumRepository.GetAlbumByAlbumId(albumId);

			if (existingAlbum == null)
				return NotFound(new { result = "album_not_found" });

			_albumRepository.RemoveAlbum(existingAlbum);

			return Ok(new { result = "album_deleted" });
		}



		[HttpGet("get-all-albums")]
		public ActionResult<IEnumerable<AlbumDTO>> GetAllAlbumsByUserId()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
				return BadRequest(new { result = "no_valid_user" });

			List<Album>? albums = _albumRepository.GetAlbumsListByUserId(user.Id);
			if (albums == null)
				return BadRequest(new { result = "no_albums_found" });

			List<AlbumDTO>? albumsDTO = new List<AlbumDTO>();
			foreach (Album album in albums)
			{
				albumsDTO.Add(new AlbumDTO
				{
					Id = album.Id,
					AlbumName = album.AlbumName,
					UserId = album.UserId
				});
			}

			return Ok(albumsDTO);
		}

		[HttpGet("get-all-photos/{albumId}")]
		public ActionResult<IEnumerable<PhotoDTO>> GetAllPhotos(Guid albumId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null || _userRepository.GetUserByToken(token) == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			Album? album = _albumRepository.GetAlbumByAlbumId(albumId);
			if (album == null)
				return BadRequest(new { result = "album_invalid" });

			List<Photo>? photos = _albumRepository.GetPhotosByAlbumId(album.Id);
			if (photos == null)
				return BadRequest(new { result = "no_photos_found" });

			List<PhotoDTO> photoDTOs = new List<PhotoDTO>();
			foreach (Photo photo in photos)
			{
				photoDTOs.Add(new PhotoDTO
				{
					Id = photo.Id,
					PhotoImageURL = photo.PhotoImageURL,
					AlbumId = photo.AlbumId,
					UploadDate = photo.UploadDate
				});
			}

			return Ok(photoDTOs);
		}

		[HttpGet("get-mini-album")]
		public ActionResult<IEnumerable<AlbumWithFirstPhoto>> GetMiniAlbum()
		{

			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? user = _userRepository.GetUserByToken(token);	
			if (user == null)
				return BadRequest(new { result = "user_invalid" });

			List<Album>? albums = _albumRepository.GetAlbumsListByUserId(user.Id);
			if (albums == null)
				return NotFound(new { result = "no_albums_found" });

			List<AlbumDTO>? albumDTOs = new List<AlbumDTO>();
			foreach (Album album in albums)
			{
				albumDTOs.Add(new AlbumDTO
				{
					Id = album.Id,
					AlbumName = album.AlbumName,
					UserId = album.UserId
				});
			}

			// Create a new list to store AlbumWithFirstPhoto objects
			List<AlbumWithFirstPhoto> albumsWithFirstPhotoList = new List<AlbumWithFirstPhoto>();

			// Iterate through each album to fetch the first photo and create AlbumWithFirstPhoto objects
			foreach (AlbumDTO albumDTO in albumDTOs)
			{
				// Fetch the first photo for the current album
				Photo? firstPhoto = _photoRepository.GetFirstPhotoForAlbum(albumDTO.Id);

				PhotoDTO firstPhotoDTO = new PhotoDTO();
				if (firstPhoto != null)
				{
					firstPhotoDTO = new PhotoDTO
					{
						Id = firstPhoto.Id,
						PhotoImageURL = firstPhoto.PhotoImageURL,
						AlbumId = firstPhoto.AlbumId,
						UploadDate = firstPhoto.UploadDate,
					};
				}

				// Create an AlbumWithFirstPhoto object that includes album information and the first photo
				AlbumWithFirstPhoto albumWithFirstPhoto = new AlbumWithFirstPhoto
				{
					AlbumDTO = albumDTO,
					FirstPhoto = firstPhotoDTO
				};

				// Add the AlbumWithFirstPhoto object to the list
				albumsWithFirstPhotoList.Add(albumWithFirstPhoto);
			}

			return Ok(albumsWithFirstPhotoList);
		}

		[HttpGet("get-uploads-album-id")]
		public ActionResult<Guid> GetUploadsAlbumId()
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });

			User? user = _userRepository.GetUserByToken(token);
			if (user == null)
				return BadRequest(new { result = "user_invalid" });


			Album? album = _albumRepository.GetUploadsAlbumId(user.Id);
			if (album == null)
				return BadRequest(new { result = "no_uploads_album" });

			return album.Id;
		}

		[HttpGet("get-album-by-id/{albumId}")]
		public ActionResult<Album> GetAlbumById(Guid albumId)
		{
			string? token = Request.Headers["Authorization"];
			if (token == null)
				return BadRequest(new { result = "no_valid_token_sent" });
			Album? album = _albumRepository.GetAlbumByAlbumId(albumId);
			if (album == null)
				return BadRequest(new { result = "no_album" });
			return album;
		}




		

	}
}
