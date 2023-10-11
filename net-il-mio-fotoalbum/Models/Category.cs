using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La categoria deve avere un nome")]
        [MaxLength(50, ErrorMessage = "Il nome della categoria può avere un massimo di 50 caratteri")]
        public string Name { get; set; }

        public List<Photo> Photos { get; set; }

        public Category()
        {

        }

    }
}
