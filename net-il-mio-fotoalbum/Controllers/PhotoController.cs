using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public IActionResult Index(string? sortOrder, string? search)
        {
            ViewBag.TitleSortParm = string.IsNullOrEmpty(sortOrder) ? "Title" : "";
            ViewBag.TitleAscSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.VisibilitySortParm = sortOrder == "Visibility" ? "Visibility_desc" : "Visibility";

            var photos = from p in _myDatabase.Photos
                                 .Include(p => p.Categories)
                                 select p;

            //filtro per ordinamento
            switch (sortOrder)
            {
                case "Title":
                    photos = photos.OrderBy(p => p.Title);
                    break;
                case "title_desc":
                    photos = photos.OrderByDescending(p => p.Title);
                    break;
                case "Visibility":
                    photos = photos.OrderBy(p => p.Visibility);
                    break;
                case "Visibility_desc":
                    photos = photos.OrderByDescending(p => p.Visibility);
                    break;
                default:
                    photos = photos.OrderBy(p => p.Title);
                    break;
            }

            //filtro per titolo

            if (!String.IsNullOrEmpty(search))
            {
                photos = photos.Where(p => p.Title.Contains(search));
            }

            return View(photos.ToList());
            //PhotoFilterModel searchedPhoto = new PhotoFilterModel();

            //if (search == null)
            //{
            //    List<Photo> photos = _myDatabase.Photos.Include(photo => photo.Categories).ToList();
            //    searchedPhoto.Photos = photos;
            //}
            //else
            //{
            //    List<Photo> photos = _myDatabase.Photos.Where(photo => photo.Title.ToLower().Contains(search.ToLower())).Include(photo => photo.Categories).ToList();
            //    searchedPhoto.Photos = photos;
            //    searchedPhoto.Filter = search;
            //}

            //return View("Index", searchedPhoto);
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Details(int id)
        {
            Photo? photoDetail = _myDatabase.Photos.Where(photo => photo.Id == id).Include(photo => photo.Categories).FirstOrDefault();

            if (photoDetail == null)
            {
                return NotFound("La foto che hai cercato non è stata trovata...");
            }
            else
            {
                return View("Details", photoDetail);
            }

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

            //imposto la visibilità
            data.Photo.Visibility = data.IsPublic;
            

            this.SetImageFileFromFormFile(data);

            _myDatabase.Photos.Add(data.Photo);
            _myDatabase.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Photo? photoToEdit = _myDatabase.Photos.Where(photo => photo.Id == id).Include(photo => photo.Categories).FirstOrDefault();

            if (photoToEdit == null)
            {
                return NotFound("La foto non è stata trovata...");
            }
            else
            {
                List<SelectListItem> categoriesSelectList = new List<SelectListItem>();
                List<Category> categories = _myDatabase.Categories.ToList();


                foreach (Category category in categories)
                {
                    categoriesSelectList.Add(new SelectListItem { 
                        Text = category.Name, 
                        Value = category.Id.ToString(), 
                        Selected = photoToEdit.Categories.Any(associatedCat => associatedCat.Id == category.Id) });
                }

                PhotoFormModel model = new PhotoFormModel { Photo = photoToEdit, Categories = categoriesSelectList, IsPublic = photoToEdit.Visibility };

                return View("Edit", model);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PhotoFormModel data)
        {
            if (!ModelState.IsValid)
            {

                List<SelectListItem> categoriesSelectList = new List<SelectListItem>();
                List<Category> categories = _myDatabase.Categories.ToList();


                foreach (Category category in categories)
                {
                    categoriesSelectList.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
                }

                data.Categories = categoriesSelectList;

                return View("Edit", data);
            }

            Photo? photoToEdit = _myDatabase.Photos.Where(photo => photo.Id == id).Include(photo => photo.Categories).FirstOrDefault();

            if (photoToEdit != null)
            {
                //svuoto la lista esistente di ingredienti
                photoToEdit.Categories.Clear();

                photoToEdit.Title = data.Photo.Title;
                photoToEdit.Description = data.Photo.Description;
                photoToEdit.Visibility = data.IsPublic;
                photoToEdit.ImageUrl = data.Photo.ImageUrl;

                if (data.SelectedCategoriesId != null)
                {
                    foreach (string categorySelected in data.SelectedCategoriesId)
                    {
                        //converto le stringhe che arrivano dal form in interi
                        int intCategorySelectedId = int.Parse(categorySelected);
                        Category? categories = _myDatabase.Categories.Where(category => category.Id == intCategorySelectedId).FirstOrDefault();

                        if (categories != null)
                        {
                            photoToEdit.Categories.Add(categories);
                        }
                    }
                }

                if (data.ImageFormFile != null)
                {
                    MemoryStream stream = new MemoryStream();
                    data.ImageFormFile.CopyTo(stream);
                    photoToEdit.ImageFile = stream.ToArray();
                }

                _myDatabase.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("La foto non è stata trovata...");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Photo? photoToDelete = _myDatabase.Photos.Where(photo => photo.Id == id).FirstOrDefault();

            if (photoToDelete != null)
            {
                _myDatabase.Photos.Remove(photoToDelete);

                _myDatabase.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("La foto non è stata trovata...");
            }

        }

        //FUNZIONE PER IL SALVATAGGIO DELLE IMMAGINI CARICATE
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
