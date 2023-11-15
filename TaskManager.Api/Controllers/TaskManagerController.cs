using Microsoft.AspNetCore.Mvc;
using TaskManager.Services.Services;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskManagerController : ControllerBase
{
    private readonly ITaskMaganerService _taskMaganerService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TaskManagerController(ITaskMaganerService taskMaganerService, IServiceScopeFactory serviceScopeFactory)
    {
        _taskMaganerService = taskMaganerService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// Get the status of the task
    /// </summary>
    /// <param name="id"></param>
    /// <response code="404">not found task</response>
    /// <response code="400">id is not a guid</response>
    [HttpGet("task/{id}")]
    public async Task<IActionResult> GetTaskStatus(string id, CancellationToken token)
    {
        if (!Guid.TryParse(id, out var guid)) return BadRequest($"this id - {id} is not a guid");

        var taskStatus = await _taskMaganerService.GetTaskStatusByIdAsync(guid, token);
        if (taskStatus == null) return NotFound($"Not found task by id {id}");

        return Ok(taskStatus);
    }
    
    /// <summary>
    /// Created empty task with imitation of work
    /// </summary>
    /// <param name="token"></param>
    /// <returns>task guid</returns>
    /// <response code="202">return task guid</response>
    [HttpPost("task")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Task(CancellationToken token)
    {
        var taskMaganerServiceScope = _serviceScopeFactory
            .CreateScope()
            .ServiceProvider
            .GetRequiredService<ITaskMaganerService>();
        
        var taskGuid = await taskMaganerServiceScope.CreateTaskDescAsync(token);
        
        const int twoMinutes = 30000;
        taskMaganerServiceScope.RunEmptyTaskByIdWithDelay(taskGuid, twoMinutes, token); //No waiting required !!!

        return Accepted(taskGuid);
    }
}