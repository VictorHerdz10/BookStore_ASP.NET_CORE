using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BookStoreApi.Conventions 
{
    public class ApiRoutePrefixConvention : IControllerModelConvention
    {
        private readonly string _prefix;

        public ApiRoutePrefixConvention(string prefix)
        {
            _prefix = prefix;
        }

        public void Apply(ControllerModel controller)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel.Template = $"{_prefix}/{selector.AttributeRouteModel.Template?.TrimStart('/')}";
                }
                else
                {
                    selector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"{_prefix}/{controller.ControllerName}"));
                }
            }
        }
    }
}