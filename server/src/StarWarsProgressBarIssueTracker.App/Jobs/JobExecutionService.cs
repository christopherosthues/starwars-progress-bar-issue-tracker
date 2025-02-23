using Polly;
using Polly.Registry;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Entities;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;
using TaskStatus = StarWarsProgressBarIssueTracker.Infrastructure.Entities.TaskStatus;

namespace StarWarsProgressBarIssueTracker.App.Jobs;

public class JobExecutionService
{
    private readonly ITaskRepository _taskRepository;
    private readonly JobFactory _jobFactory;
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;

    public JobExecutionService(ITaskRepository taskRepository, JobFactory jobFactory,
        ResiliencePipelineProvider<string> pipelineProvider, IssueTrackerContext dbContext)
    {
        _taskRepository = taskRepository;
        // _taskRepository.Context = dbContext;
        _jobFactory = jobFactory;
        _pipelineProvider = pipelineProvider;
    }

    public async Task ExecuteTask(JobType jobType, CancellationToken cancellationToken)
    {
        IEnumerable<DbTask> tasks = await _taskRepository.GetScheduledTasksAsync(jobType, cancellationToken);

        foreach (DbTask task in tasks)
        {
            if (task.Status == TaskStatus.Planned)
            {
                task.Status = TaskStatus.Running;
                await _taskRepository.UpdateAsync(task, cancellationToken);

                IJob job = _jobFactory.CreateJob(jobType);

                try
                {
                    ResiliencePipeline pipeline = _pipelineProvider.GetPipeline("job-pipeline");

                    await pipeline.ExecuteAsync(async _ =>
                    {
                        try
                        {
                            task.Status = TaskStatus.Running;
                            await _taskRepository.UpdateAsync(task, cancellationToken);

                            await job.ExecuteAsync(cancellationToken);

                            task.Status = TaskStatus.Completed;
                            task.ExecutedAt = DateTime.UtcNow;
                            await _taskRepository.UpdateAsync(task, cancellationToken);
                        }
                        catch
                        {
                            task.Status = TaskStatus.FailureWaitingForRetry;
                            await _taskRepository.UpdateAsync(task, cancellationToken);
                        }
                    }, cancellationToken);
                }
                catch
                {
                    task.Status = TaskStatus.Error;
                    await _taskRepository.UpdateAsync(task, cancellationToken);
                }
            }
        }
    }
}
