using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [MaxLength(200, ErrorMessage = "Il titolo può avere un massimo di 200 caratteri")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Il testo è obbligatorio")]
        public string Text {  get; set; }

        [Required(ErrorMessage = "L'email è obbligatorio")]
        [EmailAddress(ErrorMessage = "L'email non è valida")]
        public string Email { get; set; }

        public Message()
        {

        }
    }
}
