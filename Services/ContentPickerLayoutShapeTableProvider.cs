using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using System.Web;

namespace Lombiq.DataTables.Services
{
    public class ContentPickerLayoutShapeTableProvider : IShapeTableProvider
    {
        private readonly IHttpContextAccessor _hca;


        public ContentPickerLayoutShapeTableProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public void Discover(ShapeTableBuilder builder)
        {
            builder
                .Describe("Layout")
                .OnDisplaying(displaying =>
                {
                    if (_hca.Current().Request.IsContentPickerRequest())
                    {
                        displaying.ShapeMetadata.Alternates.Add("Layout__ContentPicker");
                    }
                });
        }
    }
}