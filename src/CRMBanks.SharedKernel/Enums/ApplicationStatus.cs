namespace CRMBanks.SharedKernel.Enums;

public enum ApplicationStatus
{
    Pending = 1,              // New application, no worker assigned
    UnderReview = 2,          // Worker has claimed it and is checking details
    ActionRequired = 3,       // Worker needs more info from client
    Approved = 4,             // Application accepted
    Rejected = 5,             // Application denied
    Escalated = 6             // Escalated to boss for review
}
