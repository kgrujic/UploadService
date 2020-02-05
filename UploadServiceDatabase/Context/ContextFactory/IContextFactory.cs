namespace UploadServiceDatabase.Context.ContextFactory
{
    public interface IContextFactory
    {
        public  UploadServiceContext CreateContext();
    }
}