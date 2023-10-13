using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private PhotoContext _myDatabase;

        public MessagesController(PhotoContext db)
        {
            _myDatabase = db;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Message newMessage)
        {
            if(newMessage == null)
            {
                return BadRequest();
            }

            _myDatabase.Messages.Add(newMessage);
            _myDatabase.SaveChanges();
            return Ok();

            
        }
    }
}
