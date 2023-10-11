using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    [Authorize(Roles = "USER,ADMIN")]
    public class PhotoController : Controller
    {
        private PhotoContext _myDatabase;

        public PhotoController(PhotoContext db)
        {
            _myDatabase = db;
        }

        public IActionResult Index()
        {
            List<Photo> photos = _myDatabase.Photos.ToList<Photo>();

            return View("Index", photos);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {
            //reperisco i dati per la select
            List<SelectListItem> categoriesSelectList = new List<SelectListItem>();
            List<Category> categories = _myDatabase.Categories.ToList();

            //popoliamo la lista
            foreach (Category category in categories)
            {
                categoriesSelectList.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
            }

            //aggiungiamo la lista al model
            PhotoFormModel model = new PhotoFormModel { Photo = new Photo(), Categories = categoriesSelectList };

            return View("Create", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel data)
        {
            if (!ModelState.IsValid)
            {
                //restituisco la lista in caso di errore
                List<SelectListItem> categoriesSelectList = new List<SelectListItem>();
                List<Category> categories = _myDatabase.Categories.ToList();

                //popoliamo la lista di ingredienti
                foreach (Category category in categories)
                {
                    categoriesSelectList.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
                }

                data.Categories = categoriesSelectList;

                return View("Create", data);
            }

            //categorie
            data.Photo.Categories = new List<Category>();

            if (data.SelectedCategoriesId != null)
            {
                foreach (string categorySelected in data.SelectedCategoriesId)
                {
                    //converto le stringhe che arrivano dal form in interi
                    int intCategorySelectedId = int.Parse(categorySelected);
                    Category? categories = _myDatabase.Categories.Where(category => category.Id == intCategorySelectedId).FirstOrDefault();

                    if (categories != null)
                    {
                        data.Photo.Categories.Add(categories);
                    }
                }
            }


            this.SetImageFileFromFormFile(data);

            _myDatabase.Photos.Add(data.Photo);
            _myDatabase.SaveChanges();

            return RedirectToAction("Index");
        }
        private void SetImageFileFromFormFile(PhotoFormModel formData)
        {
            if (formData.ImageFormFile == null)
            {
                return;
            }

            MemoryStream stream = new MemoryStream();
            formData.ImageFormFile.CopyTo(stream);
            formData.Photo.ImageFile = stream.ToArray();

        }
    }
}
