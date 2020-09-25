using static Nuke.Common.IO.FileSystemTasks;

namespace BuildSteps
{
    public abstract class BuildStep<T> where T : IBuildStepUserConfig
    {
        protected BuildStep(BuildStepConfig<T> config)
        {
            Config = config;
        }

        protected BuildStepConfig<T> Config { get; }

        protected T UserConfig => Config.BuildStepUserConfig;

        public void Execute()
        {
            // Cleaning the build step specific directory allows running a step on its own without deleting other results.
            EnsureCleanDirectory(Config.BuildStepArtifactsDirectory);

            ExecuteStep();
        }

        protected abstract void ExecuteStep();
    }
}