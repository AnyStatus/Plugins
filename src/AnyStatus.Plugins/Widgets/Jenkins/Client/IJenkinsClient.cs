using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IJenkinsClient
    {
        Task<JenkinsJob> GetJobAsync(IJenkins jenkinsPlugin);

        Task<JenkinsView> GetViewAsync(IJenkins jenkinsPlugin);

        Task TriggerJobAsync(JenkinsJob_v1 jenkinsJob);
    }
}