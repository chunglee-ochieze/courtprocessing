using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourtProcessing.Data
{
    public class Viewer
    {
        private readonly IConfiguration _config;

        public Viewer()
        {
        }

        public Viewer(IConfiguration config)
        {
            _config = config;
        }

        public List<UploadedFiles> ViewUploads()
        {
            var filesList = new List<UploadedFiles>();

            try
            {
                var permPath = _config["FilePaths:PermPath"];

                var folders = Directory.EnumerateDirectories(permPath);

                filesList.AddRange(from folder in folders
                    let files = Directory.EnumerateFiles(folder)
                    select new UploadedFiles
                    {
                        FolderName = folder.Split('\\')[Index.FromEnd(1)],
                        FolderDate = Directory.GetCreationTime(folder),
                        FileNames = string.Join(", ", files.Select(x => x.Split('\\')[Index.FromEnd(1)]).ToList()).ToLower()
                    });
            }
            catch (Exception ex)
            {
                LogHandler.WriteLog(ex.Message, LogEventLevel.Fatal, _config);
            }

            return filesList;
        }
    }
}
