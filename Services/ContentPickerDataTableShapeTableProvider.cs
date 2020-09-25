using Lombiq.DataTables.Constants;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Descriptors;
using System.Web;

namespace Lombiq.DataTables.Services
{
    public class ContentPickerDataTableShapeTableProvider : IShapeTableProvider
    {
        private readonly IHttpContextAccessor _hca;


        public ContentPickerDataTableShapeTableProvider(IHttpContextAccessor hca) => _hca = hca;


        public void Discover(ShapeTableBuilder builder) =>
            builder
                .Describe(ShapeNames.LombiqDataTable)
                .OnDisplaying(displaying =>
                {
                    if (_hca.HttpContext.Request.IsContentPickerRequest())
                    {
                        displaying.Shape.Metadata.Alternates.Add("Lombiq_ContentPicker_DataTable");
                    }
                });
    }
}
