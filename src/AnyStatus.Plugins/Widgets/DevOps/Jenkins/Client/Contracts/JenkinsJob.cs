namespace AnyStatus
{
    public class JenkinsJob
    {
        public string Name { get; set; }

        public string URL { get; set; }

        public bool Building { get; set; }

        public bool IsRunning => Building;

        public string Result { get; set; }

        public ProgressExecutor Executor { get; set; }
    }

    public class ProgressExecutor
    {
        public int Progress { get; set; }
    }
}