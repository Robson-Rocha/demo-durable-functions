using DemoDurableFunctions.SubmissionApprovalProcess.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoDurableFunctions.SubmissionApprovalProcess
{
    public static class Orchestrator
    {
        [FunctionName("SubmissionApprovalProcess_Orchestrator")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            //Gets the orchestration input
            var submission = context.GetInput<Submission>();
            submission.InstanceId = context.InstanceId;

            //For each content url, starts a new activity to download their contents
            var downloadActivityTasks = new List<Task<SubmittedContent>>();
            foreach(var submittedContent in submission.Contents)
            {
                downloadActivityTasks.Add(
                    context.CallActivityAsync<SubmittedContent>(
                        "SubmissionApprovalProcess_DownloadFileActivity", submittedContent));
            }

            //When all download activities completes, recover their contents
            await Task.WhenAll(downloadActivityTasks);
            foreach(var task in downloadActivityTasks)
            {
                SubmittedContent downloadSubmittedContent = task.Result;
                submission.Contents.First(c => c.Url == downloadSubmittedContent.Url).Content = 
                    downloadSubmittedContent.Content;
            }

            //Notifies the approver by mail to approve or decline the submission
            await context.CallActivityAsync(
                        "SubmissionApprovalProcess_SubmitForApproval", submission);

            //Waits for the approval or declining by the approver, or, if no action is taken in two minutes
            //finish the process setting the submission status as ignored
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime dueTime = context.CurrentUtcDateTime.AddMinutes(5);
                Task durableTimeout = context.CreateTimer(dueTime, timeoutCts.Token);
                Task<bool> approval = context.WaitForExternalEvent<bool>("SubmissionApprovalEvent");
                if (approval == await Task.WhenAny(approval, durableTimeout))
                {
                    timeoutCts.Cancel();
                    submission.Status = approval.Result ? "APPROVED" : "DECLINED";
                }
                else
                {
                    submission.Status = "IGNORED";
                }
            }

            //Set a custom status for this process
            context.SetCustomStatus(new { ApprovalStatus = submission.Status });

            //Notify the requester of the process completion
            await context.CallActivityAsync(
                        "SubmissionApprovalProcess_NotifyCompletionActivity", submission);

            //Return the status of the submission and completes the process
            return submission.Status;
        }
    }
}
