using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog.Events;

namespace CourtProcessing.Data
{
    public class Uploader
    {
        private readonly IConfiguration _config;

        public Uploader()
        {
        }

        public Uploader(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> UploadZip(IBrowserFile file)
        {
            string response;

            try
            {
                if (file.ContentType.ToLower() is "application/zip" or "application/x-zip-compressed")
                {
                    var maxFileSize = _config.GetValue<ushort>("MaxFileSizeMb") * 1048576;

                    if (file.Size <= maxFileSize)
                    {
                        var tempPath = _config["FilePaths:TempPath"];
                        Directory.CreateDirectory(tempPath);

                        var extArr = file.Name.Split('.');
                        var zippedFile = $"{tempPath}\\{GuidGen.TimeGen(10)}.{extArr[Index.FromEnd(1)]}";

                        await using FileStream fs = new(zippedFile, FileMode.Create);
                        await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                        fs.Close();
                        await fs.DisposeAsync();
                        
                        //confirm file availability
                        if (File.Exists(zippedFile))
                        {
                            //unzip file
                            var unzippedPath = $"{tempPath}\\{Path.GetFileNameWithoutExtension(zippedFile)}";
                            Directory.CreateDirectory(unzippedPath);
                            
                            ZipFile.ExtractToDirectory(zippedFile, unzippedPath);

                            var xmlFile = Directory.EnumerateFiles(unzippedPath)
                                .FirstOrDefault(f => Path.GetExtension(f).ToLower() is ".xml");

                            if (xmlFile is not null or "")
                            {
                                if (new Validator(_config).ValidateXmlWithSchema(xmlFile, _config["XmlSchemaPath"]))
                                {
                                    using StreamReader sr = new(xmlFile);
                                    var xElement = XElement.Parse(await sr.ReadToEndAsync());
                                    sr.Close();
                                    sr.Dispose();

                                    var name = (string)xElement.Element("name");
                                    var applicationNo = (string)xElement.Element("applicationno");
                                    var guid = GuidGen.RandomGuid().ToUpper();

                                    var permPath = $"{_config["FilePaths:PermPath"]}\\{applicationNo}-{guid}";
                                    Directory.CreateDirectory(permPath);

                                    var allowedTypes = _config["AllowedFileTypes"].Split('|');
                                    var unzippedFiles = Directory.EnumerateFiles(unzippedPath).ToList();

                                    //iterate thru each file inside the zipped file
                                    foreach (var unzippedFile in unzippedFiles.Where(f => allowedTypes.Contains(Path.GetExtension(f).Replace(".", "").ToUpper())))
                                    {
                                        File.Copy(unzippedFile, $"{permPath}\\{Path.GetFileName(unzippedFile)}");
                                    }

                                    //check that files were copied completely
                                    if (unzippedFiles.Count == Directory.EnumerateFiles(permPath).Count())
                                    {
                                        response = "Success.";
                                        new EmailHandler(_config).NotifyAdmin(new NotifyAdmin
                                        {
                                            FileExtracted = true,
                                            FolderName = permPath,
                                            UploaderApplicationNo = applicationNo,
                                            UploaderName = name
                                        });

                                        LogHandler.WriteLog($"Upload process completed successfully for Application No: {applicationNo}", LogEventLevel.Information, _config);
                                    }
                                    else
                                        response = "Files not extracted and copied completely. Please try again.";

                                    //clear temporary resources
                                    Directory.Delete(unzippedPath, true);
                                    File.Delete(zippedFile);
                                }
                                else
                                    response = "XML file not validated with XSD schema.";
                            }
                            else
                                response = $"XML file not found in zipped file {file.Name}";
                        }
                        else
                            response = "Error while saving file.";
                    }
                    else
                        response = $"Maximum allowed file size is {maxFileSize / 1048576}MB.";
                }
                else
                    response = $"{file.ContentType} not allowed.";
            }
            catch (IOException io)
            {
                LogHandler.WriteLog(io.Message, LogEventLevel.Fatal, _config);
                response = $"Unable to work on file: {file.Name}.";
            }
            catch (Exception ex)
            {
                LogHandler.WriteLog(ex.Message, LogEventLevel.Fatal, _config);
                response = $"Error while operating file: {file.Name}.";
            }

            if (response is not "Success.")
            {
                new EmailHandler(_config).NotifyAdmin(new NotifyAdmin
                {
                    FileExtracted = false
                });
            }

            return response;
        }
    }
}
