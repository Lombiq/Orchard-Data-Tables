using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace Lombiq.DataTables.Services
{
    public class ContentPickerDataTableShapeTableProvider : IShapeTableProvider
    {
        private readonly IHttpContextAccessor _hca;


        public ContentPickerDataTableShapeTableProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public void Discover(ShapeTableBuilder builder)
        {
            builder
                .Describe("Lombiq_DataTable")
                .OnDisplaying(displaying =>
                {
                    if (_hca.HttpContext.Request.IsContentPickerRequest())
                    {
                        displaying.ShapeMetadata.Alternates.Add("Lombiq_ContentPicker_DataTable");
                    }
                });
        }
    }
}