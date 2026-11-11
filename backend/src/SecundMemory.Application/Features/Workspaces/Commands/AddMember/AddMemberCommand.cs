using SecundMemory.Application.Common.Cqrs;
using SecundMemory.Application.Features.Workspaces.Dtos;
using SecundMemory.Domain.Workspaces;

namespace SecundMemory.Application.Features.Workspaces.Commands.AddMember;

public record AddMemberCommand(Guid WorkspaceId, Guid UserId, WorkspaceRole Role) : ICommand<WorkspaceDto>;
