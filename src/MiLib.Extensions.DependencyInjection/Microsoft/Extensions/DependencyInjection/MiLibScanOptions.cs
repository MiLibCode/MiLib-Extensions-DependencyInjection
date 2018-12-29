namespace Microsoft.Extensions.DependencyInjection
{
    public class MiLibScanOptions
    {
        public bool DefaultConventionsBindingOnly { get; set; }

        public MiLibScanOptions()
        {
            DefaultConventionsBindingOnly = true;
        }
    }
}