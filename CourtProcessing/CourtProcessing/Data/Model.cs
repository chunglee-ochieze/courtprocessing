using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtProcessing.Data
{
    public class MailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BC { get; set; }
        public string Subject { get; set; }
        public bool IsBodyHtml { get; set; }
        public string MailContent { get; set; }
    }

    public class NotifyAdmin
    {
        public bool FileExtracted { get; set; }
        public string UploaderName { get; set; }
        public string UploaderApplicationNo { get; set; }
        public string FolderName { get; set; }
    }

    public class UploadedFiles
    {
        public string FolderName { get; set; }
        public DateTime FolderDate { get; set; }
        public string FileNames { get; set; }
    }
}
