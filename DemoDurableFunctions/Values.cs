using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DemoDurableFunctions
{
    public static class Values
    {
        private static Dictionary<string, string> _values = new Dictionary<string, string>
        {
            {"lipsum", "Lorem ipsum dolor sit amet" },
            {"hello", "Hello Azure Functions World!" }
        };

        //[FunctionName("Values")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Values/{key}")] HttpRequest req,
            string key,
            ILogger log)
        {
            if(key == null)
                return new BadRequestObjectResult("Please provide a key in the route");

            if (!_values.ContainsKey(key))
                return new NotFoundObjectResult($"The key '{key}' was not found!");

            return new OkObjectResult(_values[key]);
        }
    }
}
