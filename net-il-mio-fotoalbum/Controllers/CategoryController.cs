using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using System.Linq;

namespace net_il_mio_fotoalbum.Controllers
{
    [Authorize(Roles = "ADMIN")]
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

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category data)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }

            _myDatabase.Categories.Add(data);
            _myDatabase.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category? categoryToEdit = _myDatabase.Categories.Where(category => category.Id == id).FirstOrDefault();

            if (categoryToEdit == null)
            {
                return NotFound("La categoria non è stata trovata...");
            }
            else
            {
                return View("Edit", categoryToEdit);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category data)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            Category? categoryToEdit = _myDatabase.Categories.Where(category => category.Id == id).FirstOrDefault();

            if (categoryToEdit != null)
            {
                categoryToEdit.Name = data.Name;

                _myDatabase.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("La categoria non è stata trovata...");
            }
        }
    }
    
}
