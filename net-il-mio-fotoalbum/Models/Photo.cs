using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace net_il_mio_fotoalbum.Models
{
    public class Photo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [MaxLength(200, ErrorMessage = "Il titolo può avere un massimo di 200 caratteri")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [Column(TypeName = "text")]
        public string Description { get; set; }


        [MaxLength(500, ErrorMessage = "Il link non può essere lungo più di 500 caratteri")]
        public string? ImageUrl { get; set; }
        public byte[]? ImageFile { get; set; }

        public string ImageSrc => ImageFile is null ? (ImageUrl is null ? "" : ImageUrl) : $"data:image/png;base64,{Convert.ToBase64String(ImageFile)}";

        [DefaultValue(true)]
        public bool Visibility { get; set; }

        //relazione n a n con le categorie
        public List<Category>? Categories { get; set; }

        public Photo()
        {

        }

        public Photo(string title, string description, bool visibility)
        {
            this.Title = title;
            this.Description = description;
            this.Visibility = visibility;
        }
    }
}
