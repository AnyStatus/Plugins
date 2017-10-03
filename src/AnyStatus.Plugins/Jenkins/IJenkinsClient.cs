using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IJenkinsClient
    {
        Task<JenkinsJob> GetJobAsync(IJenkinsPlugin jenkinsPlugin);

        Task<JenkinsView> GetViewAsync(IJenkinsPlugin jenkinsPlugin);

        Task TriggerJobAsync(JenkinsJob_v1 jenkinsJob);
    }
}
