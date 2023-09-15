using WebAPIStoreApp.Utilities.Formatters;

namespace WebAPIStoreApp.Extensions
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>

            builder.AddMvcOptions(config =>
                      config.OutputFormatters.Add(new CsvOutputFormatter()));

    }
}
