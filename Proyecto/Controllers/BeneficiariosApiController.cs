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

[ApiController]
[Authorize(Roles = IdentityRoles.DirigenteComite + "," + IdentityRoles.PersonalSubGerencia)]
[Route("api/beneficiarios")]
public sealed class BeneficiariosApiController(
    IBeneficiarioService beneficiarioService,
    IDuplicateDetectionService duplicateDetectionService,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    private const int ComiteSinAsignar = 0;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BeneficiarioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BeneficiarioDto>>> Get(
        CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiarios = await beneficiarioService.ListarAsync(alcance, cancellationToken);
        return Ok(beneficiarios.Select(Mapear));
    }

    [HttpGet("dni/{dni}")]
    [ProducesResponseType(typeof(BeneficiarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeneficiarioDto>> GetPorDni(
        string dni,
        CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiario = await beneficiarioService.ObtenerPorDniAsync(dni, alcance, cancellationToken);
        return beneficiario is null ? NotFound() : Ok(Mapear(beneficiario));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BeneficiarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BeneficiarioDto>> GetPorId(
        int id,
        CancellationToken cancellationToken)
    {
        var alcance = await ObtenerComiteAsync();
        var beneficiario = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
        return beneficiario is null ? NotFound() : Ok(Mapear(beneficiario));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BeneficiarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BeneficiarioDto>> Post(
        BeneficiarioDto dto,
        CancellationToken cancellationToken)
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Unauthorized();
        }

        var alcance = await ObtenerComiteAsync();
        if (alcance != null && dto.ComiteId != alcance)
        {
            return Forbid();
        }

        if (await duplicateDetectionService.DniActivoExisteAsync(dto.Dni, cancellationToken))
        {
            return Conflict("Ya existe una inscripción activa con el DNI indicado.");
        }

        var beneficiario = Mapear(dto);
        var registrado = await beneficiarioService.RegistrarAsync(
            beneficiario,
            usuario.Id,
            "Alta mediante API",
            cancellationToken);

        if (!registrado)
        {
            return Conflict("Ya existe una inscripción activa con el DNI indicado.");
        }

        return CreatedAtAction(nameof(GetPorId), new { id = beneficiario.Id }, Mapear(beneficiario));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put(
        int id,
        BeneficiarioDto dto,
        CancellationToken cancellationToken)
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Unauthorized();
        }

        var alcance = await ObtenerComiteAsync();
        if (alcance != null && dto.ComiteId != alcance)
        {
            return Forbid();
        }

        var actualizado = await beneficiarioService.ActualizarAsync(
            id,
            Mapear(dto),
            usuario.Id,
            "Modificacion mediante API",
            alcance,
            cancellationToken);

        if (!actualizado)
        {
            var existe = await beneficiarioService.ObtenerAsync(id, alcance, cancellationToken);
            return existe is null ? NotFound() : Conflict("El DNI ya tiene una inscripción activa.");
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/inactivar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inactivar(
        int id,
        BeneficiarioInactivarDto dto,
        CancellationToken cancellationToken)
    {
        var usuario = await userManager.GetUserAsync(User);
        if (usuario is null)
        {
            return Unauthorized();
        }

        var alcance = await ObtenerComiteAsync();
        var inactivado = await beneficiarioService.InactivarAsync(
            id,
            usuario.Id,
            dto.Motivo,
            alcance,
            cancellationToken);

        return inactivado ? NoContent() : NotFound();
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

        return usuario.ComiteId ?? ComiteSinAsignar;
    }

    private static Beneficiario Mapear(BeneficiarioDto dto) =>
        new()
        {
            Id = dto.Id,
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
            ProgramaId = beneficiario.Programas.FirstOrDefault()?.Id,
            Activo = beneficiario.Activo
        };
}
