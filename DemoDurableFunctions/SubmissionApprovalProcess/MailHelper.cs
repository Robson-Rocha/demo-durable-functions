using DemoDurableFunctions.SubmissionApprovalProcess.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDurableFunctions.SubmissionApprovalProcess
{
    public static class MailHelper
    {
        private static SendGridClient _sendGridClient =
                new SendGridClient(Environment.GetEnvironmentVariable("SendGrid_ApiKey"));

        private static string _fromMailAddress =
                Environment.GetEnvironmentVariable("FromMailAddress");

        private static string _approverMailAddress =
                Environment.GetEnvironmentVariable("ApproverMailAddress");

        public static async Task SendMailForApprover(Submission submission)
        {
            var message = new SendGridMessage();
            message.From = new EmailAddress(_fromMailAddress);
            message.AddTo(_approverMailAddress);
            message.Subject = "New content for approval";
            message.PlainTextContent = $@"
A new content was sent for your approval from {submission.SenderName} ({submission.SenderEMailAddress}):

{submission.Contents.Aggregate("", (text, submittedContent) => text += submittedContent.Content)}

To approve, click the below link:
http://localhost:7071/api/SubmissionApprovalProcess_Approve?instanceId={submission.InstanceId}

To decline, click the below link:
http://localhost:7071/api/SubmissionApprovalProcess_Decline?instanceId={submission.InstanceId}
";

            await _sendGridClient.SendEmailAsync(message);
        }

        public static async Task SendMailForSender(Submission submission)
        {
            var message = new SendGridMessage();
            message.From = new EmailAddress(_fromMailAddress);
            message.AddTo(submission.SenderEMailAddress);
            message.Subject = $"Your submission was {submission.Status}";
            message.PlainTextContent = $"The content you submitted was {submission.Status}";

            await _sendGridClient.SendEmailAsync(message);
        }
    }
}
