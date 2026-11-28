using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Modules.Autenticacion;
using Proyecto.Modules.Autenticacion.Entities;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Beneficiarios.Interfaces;
using Proyecto.Modules.Beneficiarios.Models;
using Proyecto.Modules.Duplicados.Interfaces;

namespace Proyecto.Controllers;

[Authorize(Roles = IdentityRoles.DirigenteComite + "," + IdentityRoles.PersonalSubGerencia)]
public sealed class BeneficiariosController(
    AppDbContext context,
    IBeneficiarioService beneficiarioService,
    IDuplicateDetectionService duplicateDetectionService,
    UserManager<ApplicationUser> userManager) : Controller
{
    private const int PageSize = 20;
    private const int ComiteSinAsignar = 0;

    public async Task<IActionResult> Index(
        string? dni,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var alcance = await ObtenerComiteAsync();
        if (alcance is null && !await EsPersonalAsync())
        {
            return Forbid();
        }

        dni = dni?.Trim();
        var consulta = context.Beneficiarios.AsNoTracking()
            .Include(item => item.Comite)
            .AsQueryable();
        if (alcance.HasValue)
        {
            consulta = consulta.Where(item => item.ComiteId == alcance.Value);
        }
        if (!string.IsNullOrEmpty(dni))
        {
            consulta = consulta.Where(item => item.Dni.Contains(dni));
        }

        var total = await consulta.CountAsync(cancellationToken);
        var totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPaginas);
        var beneficiarios = await consulta
            .OrderBy(item => item.Dni)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        return View(new BeneficiariosIndexViewModel
        {
            Beneficiarios = beneficiarios,
            Dni = dni,
            Pagina = page,
            TotalPaginas = totalPaginas,
            Total = total
        });
    }

    public async Task<IActionResult> Create()
    {
        var modelo = new BeneficiarioFormViewModel
        {
            Beneficiario = new BeneficiarioDto()
        };
        await PrepararFormularioAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BeneficiarioFormViewModel modelo,
        CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        if (alcance.HasValue)
        {
            modelo.Beneficiario.ComiteId = alcance.Value;
        }

        await ValidarComiteAsync(modelo.Beneficiario.ComiteId, cancellationToken);
        if (!ModelState.IsValid)
        {
            await PrepararFormularioAsync(modelo);
            return View(modelo);
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Challenge();
        }

        if (await duplicateDetectionService.DniActivoExisteAsync(
                modelo.Beneficiario.Dni,
                cancellationToken))
        {
            ModelState.AddModelError(
                "Beneficiario.Dni",
                "Ya existe una inscripción activa con este DNI. No se guardaron los datos.");
            await PrepararFormularioAsync(modelo);
            return View(modelo);
        }

        var beneficiario = Mapear(modelo.Beneficiario);
        var registrado = await beneficiarioService.RegistrarAsync(
            beneficiario,
            usuario.Id,
            "Alta desde formulario",
            cancellationToken);
        if (!registrado)
        {
            ModelState.AddModelError(
                "Beneficiario.Dni",
                "Ya existe una inscripción activa con este DNI. No se guardaron los datos.");
            await PrepararFormularioAsync(modelo);
            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiario = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
        if (beneficiario is null || !beneficiario.Activo)
        {
            return NotFound();
        }

        var modelo = new BeneficiarioFormViewModel
        {
            Beneficiario = Mapear(beneficiario)
        };
        await PrepararFormularioAsync(modelo);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        BeneficiarioFormViewModel modelo,
        CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var actual = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
        if (actual is null || !actual.Activo)
        {
            return NotFound();
        }

        if (alcance.HasValue)
        {
            modelo.Beneficiario.ComiteId = alcance.Value;
        }
        await ValidarComiteAsync(modelo.Beneficiario.ComiteId, cancellationToken);
        if (!ModelState.IsValid)
        {
            await PrepararFormularioAsync(modelo);
            return View(modelo);
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Challenge();
        }

        var actualizado = await beneficiarioService.ActualizarAsync(
            id,
            Mapear(modelo.Beneficiario),
            usuario.Id,
            "Modificacion desde formulario",
            alcance,
            cancellationToken);
        if (!actualizado)
        {
            ModelState.AddModelError(
                "Beneficiario.Dni",
                "El DNI ya tiene una inscripción activa. No se guardaron los cambios.");
            await PrepararFormularioAsync(modelo);
            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(
        int id,
        BeneficiarioInactivarDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Debe indicar un motivo de inactivación (máximo 500 caracteres).";
            return RedirectToAction(nameof(Index));
        }

        var alcance = await ObtenerComiteAsync();
        var beneficiario = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
        if (beneficiario is null)
        {
            return NotFound();
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Challenge();
        }

        var inactivado = await beneficiarioService.InactivarAsync(
            id,
            usuario.Id,
            dto.Motivo,
            alcance,
            cancellationToken);
        if (!inactivado)
        {
            return NotFound();
        }

        TempData["Mensaje"] = "El beneficiario fue inactivado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Historial(int id, CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiario = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
        if (beneficiario is null)
        {
            return NotFound();
        }

        var historial = await (
            from cambio in context.HistorialCambios.AsNoTracking()
            join usuario in context.Users.AsNoTracking()
                on cambio.UsuarioId equals usuario.Id into usuarios
            from usuario in usuarios.DefaultIfEmpty()
            where cambio.BeneficiarioId == id
            orderby cambio.FechaUtc descending
            select new HistorialCambioViewModel
            {
                Usuario = usuario == null ? "Sistema" : usuario.UserName ?? "Usuario",
                Fecha = cambio.FechaUtc,
                Operacion = cambio.Operacion,
                Motivo = cambio.Motivo
            }).ToListAsync(cancellationToken);

        return View(new HistorialBeneficiarioViewModel
        {
            Beneficiario = beneficiario,
            Cambios = historial
        });
    }

    private async Task<int?> ObtenerComiteAsync()
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null
            || await userManager.IsInRoleAsync(usuario, IdentityRoles.PersonalSubGerencia))
        {
            return null;
        }

        return await userManager.IsInRoleAsync(usuario, IdentityRoles.DirigenteComite)
            ? usuario.ComiteId ?? ComiteSinAsignar
            : null;
    }

    private async Task<bool> EsPersonalAsync()
    {
        var usuario = await userManager.GetUserAsync(User);
        return usuario is not null
            && await userManager.IsInRoleAsync(usuario, IdentityRoles.PersonalSubGerencia);
    }

    private async Task ValidarComiteAsync(int comiteId, CancellationToken cancellationToken)
    {
        if (!await context.Comites.AnyAsync(item => item.Id == comiteId, cancellationToken))
        {
            ModelState.AddModelError("Beneficiario.ComiteId", "Seleccione un comité válido.");
        }
    }

    private async Task PrepararFormularioAsync(
        BeneficiarioFormViewModel modelo,
        CancellationToken cancellationToken = default)
    {
        var alcance = await ObtenerComiteAsync();
        var comites = context.Comites.AsNoTracking().AsQueryable();
        if (alcance.HasValue)
        {
            comites = comites.Where(item => item.Id == alcance.Value);
            modelo.Beneficiario.ComiteId = alcance.Value;
        }

        modelo.Comites = await comites
            .OrderBy(item => item.Nombre)
            .Select(item => new SelectListItem(item.Nombre, item.Id.ToString()))
            .ToListAsync(cancellationToken);
    }

    private static Beneficiario Mapear(BeneficiarioDto dto) =>
        new()
        {
            Dni = dto.Dni,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Direccion = dto.Direccion,
            ComiteId = dto.ComiteId,
            Activo = dto.Activo
        };

    private static BeneficiarioDto Mapear(Beneficiario beneficiario) =>
        new()
        {
            Id = beneficiario.Id,
            Dni = beneficiario.Dni,
            Nombres = beneficiario.Nombres,
            Apellidos = beneficiario.Apellidos,
            Direccion = beneficiario.Direccion,
            ComiteId = beneficiario.ComiteId,
            Activo = beneficiario.Activo
        };
}
