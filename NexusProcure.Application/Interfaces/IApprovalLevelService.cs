using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces;

public interface IApprovalLevelService
{
    Task<ApprovalLevelResponseDto> CreateAsync(ApprovalLeveRequestlDto dto);
    Task<List<ApprovalLevelResponseDto>> GetAllAsync();
}