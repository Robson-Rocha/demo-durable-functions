using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DemoDurableFunctions.SubmissionApprovalProcess.Functions
{
    public static class ApproveFunction
    {
        [FunctionName("SubmissionApprovalProcess_Approve")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            string instanceId = req.Query["instanceId"];
            var status = await client.GetStatusAsync(instanceId);
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                await client.RaiseEventAsync(instanceId, "SubmissionApprovalEvent", true);
                return new OkObjectResult("Submission approved");
            }
            return new OkObjectResult($"This submission can no longer be approved as its approval process is {status.RuntimeStatus} and the submission has been {status.CustomStatus?.Value<string>("ApprovalStatus") ?? "abandoned"}.");

        }
    }
}
