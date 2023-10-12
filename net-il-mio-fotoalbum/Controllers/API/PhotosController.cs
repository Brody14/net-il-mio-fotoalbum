using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using System.Collections.Generic;

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
        public IActionResult GetPhotoByName(string? search)
        {
            List<Photo> searchedPhoto;

            if (search == null)
            {
                searchedPhoto = _myDatabase.Photos.Include(photo => photo.Categories).ToList(); ;
            }
            else
            {
                searchedPhoto = _myDatabase.Photos.Where(photo => photo.Title.ToLower().Contains(search.ToLower())).Include(photo => photo.Categories).ToList();
            }
            

            return Ok(searchedPhoto);
        }
    }
}
