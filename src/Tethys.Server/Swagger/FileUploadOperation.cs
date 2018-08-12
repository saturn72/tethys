using System;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tethys.Server.Swagger
{
    internal class FileUploadOperation : IOperationFilter
    {
        static readonly IEnumerable<string> fileUploadOperationIds = new[] {
        Consts.MockControllerRoute.Replace("/", "") + "uploadpost"};
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!fileUploadOperationIds.Contains(operation.OperationId, StringComparer.InvariantCultureIgnoreCase))
                return;

            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();
            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "files",
                In = "formData",
                Description = "Upload File",
                Required = true,
                Type = "file"
            });
            operation.Consumes.Add("multipart/form-data");
        }
    }
}