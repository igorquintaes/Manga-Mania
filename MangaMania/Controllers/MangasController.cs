using MangaMania.Database.Contexts;
using MangaMania.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MangaMania.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class MangasController : ControllerBase
    {
        protected readonly ILogger<MangasController> logger;
        protected readonly AppDbContext context;

        public MangasController(ILogger<MangasController> logger, AppDbContext context)
            => (this.logger, this.context) = (logger, context);

        [HttpGet("{id}")]
        public async Task<ActionResult<Manga>> Get(int id) =>
            await context.Mangas.FirstOrDefaultAsync(x => x.Id == id);
    }
}
