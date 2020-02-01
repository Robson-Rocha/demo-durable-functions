using DemoDurableFunctions.SubmissionApprovalProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoDurableFunctions.SubmissionApprovalProcess.Activities
{
    public static class DownloadFileActivity
    {
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("SubmissionApprovalProcess_DownloadFileActivity")]
        public static async Task<SubmittedContent> DownloadFile(
            [ActivityTrigger] SubmittedContent submittedContent)
        {
            submittedContent.Content = await _httpClient.GetStringAsync(submittedContent.Url);
            return submittedContent;
        }
    }
}
