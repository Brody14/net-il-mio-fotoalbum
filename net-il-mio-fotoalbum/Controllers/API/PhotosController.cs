using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private PhotoContext _myDatabase;

        public PhotosController(PhotoContext db)
        {
            _myDatabase = db;
        }

        [HttpGet]
        public IActionResult GetPhotos()
        {
            List<Photo> photos = _myDatabase.Photos.Include(photo => photo.Categories).ToList();

            return Ok(photos);
        }

        [HttpGet]
        public IActionResult GetPhotoByName(string search)
        {
            List<Photo> searchedPhoto = _myDatabase.Photos.Where(photo => photo.Title.ToLower().Contains(search.ToLower())).ToList();

            return Ok(searchedPhoto);
        }
    }
}
