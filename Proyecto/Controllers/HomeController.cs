using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Modules.Beneficiarios.Models;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class HomeController(AppDbContext context) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var conteos = await context.Beneficiarios
                .AsNoTracking()
                .Where(item => item.Activo)
                .GroupBy(item => item.ComiteId)
                .Select(grupo => new { ComiteId = grupo.Key, Total = grupo.Count() })
                .ToDictionaryAsync(item => item.ComiteId, item => item.Total, cancellationToken);
            var comites = await context.Comites
                .AsNoTracking()
                .OrderBy(item => item.Nombre)
                .Select(item => new { item.Id, item.Nombre })
                .ToListAsync(cancellationToken);
            var porComite = comites.Select(comite => new ResumenComiteViewModel
            {
                Comite = comite.Nombre,
                Total = conteos.GetValueOrDefault(comite.Id)
            }).ToList();

            return View(new ResumenBeneficiariosViewModel
            {
                TotalActivos = porComite.Sum(item => item.Total),
                PorComite = porComite
            });
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
