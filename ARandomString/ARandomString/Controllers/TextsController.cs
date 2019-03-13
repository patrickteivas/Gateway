using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ARandomString.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            Console.WriteLine("Minult asdasdas");
            var rnd = new Random();

            return new string[] { "value1", "value2", "value" + rnd.Next()*10000 };
        }
    }
}
