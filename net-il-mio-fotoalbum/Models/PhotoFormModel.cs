using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public Photo Photo { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<string>? SelectedCategoriesId { get; set; }
        public IFormFile? ImageFormFile { get; set; }

        public bool IsPublic { get; set; }
    }
}
