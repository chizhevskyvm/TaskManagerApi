using AutoMapper;
using TaskManager.Services.DTOs;
using TaskManager.Services.Enums;
using TaskManagerApi.Model;
using TaskManagerApi.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Services.Services.Impl;

public class TaskMaganerService : ITaskMaganerService
{
    private readonly TaskManagerDbContext _context;
    private readonly IMapper _mapper;
    
    public TaskMaganerService(TaskManagerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<Guid> CreateTaskDescAsync(CancellationToken cts)
    {
        var taskNew = new TaskDescEntity
        {
            Id = Guid.NewGuid(),
            UpdateTime = DateTime.Now,
            Status = (int)TaskDescEnitityStatus.Created
        };

        await _context.AddAsync(taskNew, cts);
        await _context.SaveChangesAsync(cts);

        return taskNew.Id;
    }

    /// <summary>
    /// launch a blank task with a change in the Running to Finished status
    /// </summary>
    /// <param name="id"></param>
    /// <param name="millisecondsTimeout"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public void RunEmptyTaskByIdWithDelay(Guid id, int millisecondsTimeout, CancellationToken cts)
    {
        Task.Run(() =>
        {
            var taskToRun = _context.TaskEntities.Find(id);
            if (taskToRun == null) throw new($"Entity {typeof(TaskDescEntity)} not found.");

            ChangeTaskDescBody(_context, taskToRun, TaskDescEnitityStatus.Running);
            
            Thread.Sleep(millisecondsTimeout);
            
            ChangeTaskDescBody(_context, taskToRun, TaskDescEnitityStatus.Finished);
        }, cts);
    }

    public async Task<TaskDescStatusDto?> GetTaskStatusByIdAsync(Guid id, CancellationToken cts)
    {
        var task = await _context.TaskEntities
            .FirstOrDefaultAsync(x => x.Id == id, cts);
        return task == null ? null : _mapper.Map<TaskDescStatusDto>(task);
    }

    private static void ChangeTaskDescBody(DbContext context, TaskDescEntity taskDescEntity, TaskDescEnitityStatus descEnitityStatus)
    {
        taskDescEntity.Status = (int)descEnitityStatus;
        taskDescEntity.UpdateTime = DateTime.Now;
        
        context.SaveChanges();
    }
}