namespace BuildSteps
{
    public interface IBuildStepUserConfig
    {
        public string StepName { get; }

        public bool Enabled { get; }
    }
}