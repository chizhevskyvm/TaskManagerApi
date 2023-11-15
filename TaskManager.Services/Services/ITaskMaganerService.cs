using TaskManager.Services.DTOs;

namespace TaskManager.Services.Services;

public interface ITaskMaganerService
{
    Task<Guid> CreateTaskDescAsync(CancellationToken cts);
    Task<TaskDescStatusDto> GetTaskStatusByIdAsync(Guid id, CancellationToken cts);
    void RunEmptyTaskByIdWithDelay(Guid id, int millisecondsTimeout, CancellationToken cts);
}