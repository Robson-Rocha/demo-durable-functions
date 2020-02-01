using DemoDurableFunctions.SubmissionApprovalProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoDurableFunctions.SubmissionApprovalProcess
{
    public static class Starter
    {
        [FunctionName("SubmissionApprovalProcess")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter)
        {
            //Gets the submission payload
            Submission submission = await req.Content.ReadAsAsync<Submission>();

            //Starts a new orchestration
            string instanceId = await starter.StartNewAsync("SubmissionApprovalProcess_Orchestrator", submission);

            //Returns the check status response
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
