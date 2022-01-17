namespace Components.Analyzer
{
    public interface IUserConfig
    {
        public string StepName { get; }

        public bool Enabled { get; }
    }
}