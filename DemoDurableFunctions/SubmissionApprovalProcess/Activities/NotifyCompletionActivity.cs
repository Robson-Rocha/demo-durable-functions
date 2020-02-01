using DemoDurableFunctions.SubmissionApprovalProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace DemoDurableFunctions.SubmissionApprovalProcess.Activities
{
    public static class NotifyCompletionActivity
    {
        [FunctionName("SubmissionApprovalProcess_NotifyCompletionActivity")]
        public async static Task NotifyCompletion([ActivityTrigger] Submission submission)
        {
            await MailHelper.SendMailForSender(submission);
        }
    }
}
