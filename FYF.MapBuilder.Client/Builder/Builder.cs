namespace FYF.MapBuilder.Client
{
    internal class Builder
    {
        public BuilderObjectManager BuilderObjectManager { get; private set; }

        public Builder()
        {
            BuilderObjectManager = new BuilderObjectManager();
        }
    }
}
