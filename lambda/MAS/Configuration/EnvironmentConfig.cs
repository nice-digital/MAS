namespace MAS.Configuration
{
    public class EnvironmentConfig
    {
        public static EnvironmentConfig Current { get; private set; }

        public EnvironmentConfig()
        {
            Current = this;
        }

        public string Name { get; set; }
    }
}
