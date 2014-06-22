
namespace MediaSync.Model
{
    public class SyncCopyResult
    {
        public int CopiedSuccessfullyCount { get; set; }
        public int AlreadyExistedCount { get; set; }
        public int CopiedButAlreadyExistedDiffSizeCount { get; set; }
        public int UncopiedProblemCount { get; set; }
    }
}
