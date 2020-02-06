namespace UploadServiceDatabase.Context.ContextFactory
{
    /// <summary>
    /// IContextFactory is interface for Context Factory
    /// </summary>
    public interface IContextFactory
    {
        public  UploadServiceContext CreateContext();
    }
}