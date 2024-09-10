using Polly.Registry;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;
using StarWarsProgressBarIssueTracker.Infrastructure.Repositories;

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
        var tasks = await _taskRepository.GetScheduledTasksAsync(jobType, cancellationToken);

        foreach (var task in tasks)
        {
            if (task.Status == Infrastructure.Models.TaskStatus.Planned)
            {
                task.Status = Infrastructure.Models.TaskStatus.Running;
                await _taskRepository.UpdateAsync(task, cancellationToken);

                var job = _jobFactory.CreateJob(jobType);

                try
                {
                    var pipeline = _pipelineProvider.GetPipeline("job-pipeline");

                    await pipeline.ExecuteAsync(async _ =>
                    {
                        try
                        {
                            task.Status = Infrastructure.Models.TaskStatus.Running;
                            await _taskRepository.UpdateAsync(task, cancellationToken);

                            await job.ExecuteAsync(cancellationToken);

                            task.Status = Infrastructure.Models.TaskStatus.Completed;
                            task.ExecutedAt = DateTime.UtcNow;
                            await _taskRepository.UpdateAsync(task, cancellationToken);
                        }
                        catch
                        {
                            task.Status = Infrastructure.Models.TaskStatus.FailureWaitingForRetry;
                            await _taskRepository.UpdateAsync(task, cancellationToken);
                        }
                    }, cancellationToken);
                }
                catch
                {
                    task.Status = Infrastructure.Models.TaskStatus.Error;
                    await _taskRepository.UpdateAsync(task, cancellationToken);
                }
            }
        }
    }
}
