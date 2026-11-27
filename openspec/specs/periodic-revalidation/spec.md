# periodic-revalidation Specification

## Purpose

Mantener vigente la información de los beneficiarios mediante revalidaciones periódicas.

## Requirements

### Requirement: Programar revalidación
El sistema SHALL solicitar la revalidación de cada beneficiario cada seis meses.

#### Scenario: Periodo vencido
- **WHEN** transcurre el periodo de revalidación configurado
- **THEN** el sistema genera una solicitud pendiente y notifica al comité correspondiente
