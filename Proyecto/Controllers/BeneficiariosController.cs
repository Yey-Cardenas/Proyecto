using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Modules.Autenticacion;
using Proyecto.Modules.Autenticacion.Entities;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Beneficiarios.Interfaces;
using Proyecto.Modules.Beneficiarios.Models;
using Proyecto.Modules.Duplicados.Interfaces;

namespace Proyecto.Controllers;

[Authorize]
public sealed class BeneficiariosController(
    IBeneficiarioService beneficiarioService,
    IDuplicateDetectionService duplicateDetectionService,
    UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiarios = await beneficiarioService.ListarAsync(alcance, cancellationToken);
        return View(beneficiarios);
    }

    public IActionResult Create() => View(new BeneficiarioDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BeneficiarioDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Challenge();
        }

        var alcance = await ObtenerComiteAsync();
        if (alcance != null && dto.ComiteId != alcance)
        {
            return Forbid();
        }

        if (await duplicateDetectionService.DniActivoExisteAsync(dto.Dni, cancellationToken))
        {
            ModelState.AddModelError(nameof(dto.Dni), "Ya existe una inscripción activa con este DNI.");
            return View(dto);
        }

        var beneficiario = Mapear(dto);
        var registrado = await beneficiarioService.RegistrarAsync(
            beneficiario,
            usuario.Id,
            "Alta desde formulario",
            cancellationToken);

        if (!registrado)
        {
            ModelState.AddModelError(nameof(dto.Dni), "Ya existe una inscripción activa con este DNI.");
            return View(dto);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiario = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
        return beneficiario is null ? NotFound() : View(Mapear(beneficiario));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        BeneficiarioDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Challenge();
        }

        var alcance = await ObtenerComiteAsync();
        var actualizado = await beneficiarioService.ActualizarAsync(
            id,
            Mapear(dto),
            usuario.Id,
            "Modificacion desde formulario",
            alcance,
            cancellationToken);

        return actualizado ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inactivar(
        int id,
        string motivo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(motivo))
        {
            ModelState.AddModelError(string.Empty, "Debe indicar el motivo de inactivación.");
            return RedirectToAction(nameof(Index));
        }

        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Challenge();
        }

        var alcance = await ObtenerComiteAsync();
        await beneficiarioService.InactivarAsync(
            id,
            usuario.Id,
            motivo,
            alcance,
            cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> ObtenerComiteAsync()
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return null;
        }

        if (await userManager.IsInRoleAsync(usuario, IdentityRoles.PersonalSubGerencia))
        {
            return null;
        }

        return usuario.ComiteId;
    }

    private static Beneficiario Mapear(BeneficiarioDto dto) =>
        new()
        {
            Dni = dto.Dni,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Direccion = dto.Direccion,
            ComiteId = dto.ComiteId,
            Activo = dto.Activo,
            Programas = dto.ProgramaId.HasValue
                ? [new Programa { Id = dto.ProgramaId.Value, Nombre = string.Empty }]
                : []
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
