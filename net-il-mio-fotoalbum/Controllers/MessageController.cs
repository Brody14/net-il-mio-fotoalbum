using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class MessageController : Controller
    {
       
        private PhotoContext _myDatabase;

        public MessageController(PhotoContext db)
        {
            _myDatabase = db;
        }

        public IActionResult Index()
        {
            List<Message> messages = _myDatabase.Messages.ToList<Message>();

            return View("Index", messages);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Message? messageToDelete = _myDatabase.Messages.Where(message => message.Id == id).FirstOrDefault();

            if (messageToDelete != null)
            {
                _myDatabase.Messages.Remove(messageToDelete);

                _myDatabase.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("Il messaggio non è stata trovata...");
            }
        }
    }
}
