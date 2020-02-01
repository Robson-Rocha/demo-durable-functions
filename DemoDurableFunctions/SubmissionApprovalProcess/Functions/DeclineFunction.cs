using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace DemoDurableFunctions.SubmissionApprovalProcess.Functions
{
    public static class DeclineFunction
    {
        [FunctionName("SubmissionApprovalProcess_Decline")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            string instanceId = req.Query["instanceId"];
            var status = await client.GetStatusAsync(instanceId);
            if (status.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                await client.RaiseEventAsync(instanceId, "SubmissionApprovalEvent", false);
                return new OkObjectResult("Submission declined");
            }
            return new OkObjectResult($"This submission can no longer be declined as its approval process is {status.RuntimeStatus} and the submission has been {status.CustomStatus?.Value<string>("ApprovalStatus") ?? "abandoned"}.");
        }
    }
}
