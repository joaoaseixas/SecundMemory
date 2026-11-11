# Mappings

Per [ADR 0003](../../../../../docs/adr/0003-custom-mapping-over-automapper.md), there is
no AutoMapper `Profile` here. Each feature's DTO carries its own
`ToDto(this Entity entity)` extension method, defined right next to the DTO
in its feature folder (e.g. `Features/Pages/Dtos/PageDto.cs`).
