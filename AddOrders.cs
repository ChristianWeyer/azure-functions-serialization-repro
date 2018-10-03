
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Serverless
{
    /*
    This is the JSON being used to POST for testing:
    {
	"description": "A",
	"items": [
		{
			"name": "B",
			"description": "C"
		}	
	]}
    */
    public static class AddOrders
    {
        [FunctionName("AddOrdersModel")]
        public static async Task<IActionResult> AddOrdersModel(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "orders")]
            Order newOrder,
            ILogger log)
        {
            log.LogInformation("***AddOrdersModel processed a request: {0}",
                JsonConvert.SerializeObject(newOrder));

            if (newOrder.Items == null)
            {
                return new BadRequestResult();
            }

            return new OkResult();
        }

        [FunctionName("AddOrdersRaw")]
        public static async Task<IActionResult> AddOrdersRaw(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "ordersraw")]
            HttpRequest req,
            ILogger log)
        {
            var newOrder = req.DeserializeModel<Order>();

            log.LogInformation("***AddOrdersRaw processed a request: {0}",
                JsonConvert.SerializeObject(newOrder));

            if (newOrder.Items == null)
            {
                return new BadRequestResult();
            }

            return new OkResult();
        }

        private static T DeserializeModel<T>(this HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            using (var textReader = new JsonTextReader(reader))
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                var serializer = JsonSerializer.Create(new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                return serializer.Deserialize<T>(textReader);
            }
        }
    }
}
