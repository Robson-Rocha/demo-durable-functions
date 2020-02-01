using DemoDurableFunctions.SubmissionApprovalProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace DemoDurableFunctions.SubmissionApprovalProcess.Activities
{
    public static class SubmitForApprovalActivity
    {
        [FunctionName("SubmissionApprovalProcess_SubmitForApproval")]
        public async static Task SubmitForApproval([ActivityTrigger] Submission submission)
        {
            await MailHelper.SendMailForApprover(submission);
        }
    }
}
