namespace DemoDurableFunctions.SubmissionApprovalProcess.Models
{
    public class SubmittedContent
    {
        public string Url { get; set; }
        public string Content { get; set; }
    }

    public class Submission
    {
        public string SenderName { get; set; }
        public string SenderEMailAddress { get; set; }
        public string InstanceId { get; set; }
        public SubmittedContent[] Contents { get; set; }
        public string Status { get; set; }
    }
}
