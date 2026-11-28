using System.Data;
using Microsoft.Extensions.Caching.Memory;
using Proyecto.Data.Interfaces;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Beneficiarios.Interfaces;
using Proyecto.Shared.Auditoria;

namespace Proyecto.Modules.Beneficiarios.Services;

public sealed class BeneficiarioService(
    IUnitOfWork unitOfWork,
    IAuditoriaService auditoriaService,
    IMemoryCache? memoryCache = null) : IBeneficiarioService
{
    private const string CachePrefix = "beneficiario:dni:";

    public Task<List<Beneficiario>> ListarAsync(
        int? comiteId,
        CancellationToken cancellationToken = default) =>
        unitOfWork.Repository<Beneficiario>().ListAsync(
            beneficiario => comiteId == null || beneficiario.ComiteId == comiteId,
            cancellationToken);

    public Task<Beneficiario?> ObtenerAsync(
        int id,
        int? comiteId,
        CancellationToken cancellationToken = default) =>
        unitOfWork.Repository<Beneficiario>().GetByIdAsync(cancellationToken, id)
            .ContinueWith(task =>
            {
                var beneficiario = task.Result;
                return comiteId == null || beneficiario?.ComiteId == comiteId
                    ? beneficiario
                    : null;
            }, cancellationToken);

    public Task<Beneficiario?> ObtenerPorDniAsync(
        string dni,
        int? comiteId,
        CancellationToken cancellationToken = default)
    {
        ValidarDni(dni);
        return unitOfWork.Repository<Beneficiario>().ListAsync(
                beneficiario => beneficiario.Dni == dni
                    && (comiteId == null || beneficiario.ComiteId == comiteId),
                cancellationToken)
            .ContinueWith(task => task.Result.FirstOrDefault(), cancellationToken);
    }

    public async Task<bool> RegistrarAsync(
        Beneficiario beneficiario,
        string usuarioId,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        ValidarDni(beneficiario.Dni);
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);

        await using var transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken,
            IsolationLevel.Serializable);

        var repository = unitOfWork.Repository<Beneficiario>();
        if (await repository.AnyAsync(
                existente => existente.Dni == beneficiario.Dni && existente.Activo,
                cancellationToken))
        {
            return false;
        }

        beneficiario.Activo = true;

        await repository.AddAsync(beneficiario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditoriaService.RegistrarCambioAsync(
            new AuditEntry(usuarioId, beneficiario.Id, "Alta", motivo),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        memoryCache?.Remove(CachePrefix + beneficiario.Dni);
        return true;
    }

    public async Task<bool> ActualizarAsync(
        int id,
        Beneficiario datos,
        string usuarioId,
        string motivo,
        int? comiteId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);
        if (string.IsNullOrWhiteSpace(motivo))
        {
            return false;
        }
        ValidarDni(datos.Dni);

        await using var transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken,
            IsolationLevel.Serializable);

        var repository = unitOfWork.Repository<Beneficiario>();
        var existente = await repository.GetByIdAsync(cancellationToken, id);
        if (existente is null
            || !existente.Activo
            || (comiteId != null && existente.ComiteId != comiteId))
        {
            return false;
        }

        if (!string.Equals(existente.Dni, datos.Dni, StringComparison.Ordinal)
            && await repository.AnyAsync(
                beneficiario => beneficiario.Dni == datos.Dni && beneficiario.Activo,
                cancellationToken))
        {
            return false;
        }

        existente.Dni = datos.Dni;
        existente.Nombres = datos.Nombres;
        existente.Apellidos = datos.Apellidos;
        existente.Direccion = datos.Direccion;
        existente.ComiteId = datos.ComiteId;
        if (datos.Programas.Count > 0)
        {
            existente.Programas = datos.Programas;
        }

        repository.Update(existente);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditoriaService.RegistrarCambioAsync(
            new AuditEntry(usuarioId, existente.Id, "Actualizacion", motivo),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        memoryCache?.Remove(CachePrefix + existente.Dni);
        return true;
    }

    public async Task<bool> InactivarAsync(
        int id,
        string usuarioId,
        string motivo,
        int? comiteId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);
        if (string.IsNullOrWhiteSpace(motivo))
        {
            return false;
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken,
            IsolationLevel.Serializable);

        var repository = unitOfWork.Repository<Beneficiario>();
        var existente = await repository.GetByIdAsync(cancellationToken, id);
        if (existente is null
            || !existente.Activo
            || (comiteId != null && existente.ComiteId != comiteId))
        {
            return false;
        }

        existente.Activo = false;
        repository.Update(existente);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await auditoriaService.RegistrarCambioAsync(
            new AuditEntry(usuarioId, existente.Id, "Inactivacion", motivo),
            cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        memoryCache?.Remove(CachePrefix + existente.Dni);
        return true;
    }

    private static void ValidarDni(string dni)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);
        if (dni.Length != 8 || !dni.All(char.IsAsciiDigit))
        {
            throw new ArgumentException("El DNI debe contener exactamente 8 digitos.", nameof(dni));
        }
    }
}
