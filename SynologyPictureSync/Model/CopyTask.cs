using System;

namespace MediaSync 
{
    public class CopyTask
    {
        public string SourceFile { get; set; }
        public string DestinationFile { get; set; }
        public DateTime FileCreatedOn { get; set; }
        public DateTime FileCopiedOn { get; set; }
        public string CopyStatus { get; set; }
        public CopyResult CopyResult { get; set; }
        public DateTime? ImageTakenOnDate { get; set; }
    }

    public enum CopyResult { AlreadyExisted, CopiedSuccessfully, AlreadyExistedSizeMismatch }

}
