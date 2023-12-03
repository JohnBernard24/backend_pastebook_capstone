using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_pastebook_capstone.Controllers
{
	[Route("api/photo")]
	[ApiController]
	public class PhotoController : ControllerBase
	{


		private readonly PhotoRepository _photoRepository;
		private readonly AlbumRepository _albumRepository;
		private readonly UserRepository _userRepository;


		public PhotoController(PhotoRepository photoRepository, AlbumRepository albumRepository, UserRepository userRepository)
		{
			_photoRepository = photoRepository;
			_albumRepository = albumRepository;
			_userRepository = userRepository;
		}


		[HttpPost("add-photo")]
		public async Task<IActionResult> AddPhoto([FromForm] Guid albumId, [FromForm] IFormFile file)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			try
			{
				if (file == null || file.Length == 0)
				{
					return BadRequest("file_sent_empty");
				}

				string uploadsFolder = Path.Combine("..", "..", "PastebookData", "photos", albumId.ToString());
				string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);

				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}

				Album? album = _albumRepository.GetAlbumByAlbumId(albumId);
				string imageUrl = $"photos/{albumId}/{uniqueFileName}";

				Photo photo = new Photo
				{
					PhotoImageURL = imageUrl,
					AlbumId = albumId,
					Album = album
				};

				_photoRepository.AddPhoto(photo);

				return Ok(photo.Id);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}


		[HttpGet("get-photo/{photoId}")]
		public IActionResult GetPhoto(Guid photoId)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			Photo? existingPhoto = _photoRepository.GetPhotoByPhotoId(photoId);
			if (existingPhoto == null)
			{
				return BadRequest(new { result = "photo_notFound" });
			}

			try
			{
				string imagePath = Path.Combine("..", "..", "PastebookData", existingPhoto.PhotoImageURL.TrimStart('/'));

				if (!System.IO.File.Exists(imagePath))
				{
					return NotFound("photo_file_not_found");
				}

				byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

				// Determine content type based on file extension or use a default value
				string contentType = GetContentType(imagePath);

				return File(imageBytes, contentType);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		private string GetContentType(string filePath)
		{
			string? extension = Path.GetExtension(filePath)?.ToLowerInvariant();

			switch (extension)
			{
				case ".jpg":
				case ".jpeg":
					return "image/jpeg";
				case ".png":
					return "image/png";
				case ".gif":
					return "image/gif";
				case ".bmp":
					return "image/bmp";
				// Add more cases for other file types as needed
				default:
					return "application/octet-stream"; // Default to binary if type is unknown
			}
		}


		[HttpDelete("delete-photo/{photoId}")]
		public IActionResult DeletePhoto(Guid photoId)
		{
			string? token = Request.Headers["Authorization"];

			if (token == null || _userRepository.GetUserByToken(token) == null)
			{
				return BadRequest(new { result = "no_valid_token_sent" });
			}

			try
			{
				Photo? photo = _photoRepository.GetPhotoByPhotoId(photoId);

				if (photo == null)
				{
					return NotFound(new {result = "photo_not_found" });
				}

				string imagePath = Path.Combine("..", "PastebookData", photo.PhotoImageURL.TrimStart('/'));

				System.IO.File.Delete(imagePath);

				_photoRepository.RemovePhoto(photo);

				return Ok(new {result = "photo_deleted_successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

	}
}
