using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using System.Linq;

namespace net_il_mio_fotoalbum.Controllers
{
    public class CategoryController : Controller
    {
        private PhotoContext _myDatabase;

        public CategoryController(PhotoContext db)
        {
            _myDatabase = db;
        }

        public IActionResult Index()
        {
            List<Category> categories = _myDatabase.Categories.ToList<Category>();

            return View("Index", categories);
        }
    }
}
