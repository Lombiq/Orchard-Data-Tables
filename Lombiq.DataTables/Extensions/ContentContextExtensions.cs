namespace OrchardCore.ContentManagement.Handlers
{
    public static class ContentContextExtensions
    {
        public static bool IsRemove(this ContentContextBase context) =>
            context is RemoveContentContext || context?.ContentItem?.Latest == false;
    }
}
