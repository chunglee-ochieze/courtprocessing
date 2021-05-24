using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CourtProcessing.Data
{
    public class EmailHandler
    {
        private readonly IConfiguration _config;

        public EmailHandler()
        {
        }

        public EmailHandler(IConfiguration config)
        {
            _config = config;
        }

        public void NotifyAdmin(NotifyAdmin notify)
        {
            try
            {
                var sent = SendEmail(new MailModel
                {
                    From = _config["Email:From"],
                    To = _config["Email:To"],
                    CC = _config["Email:Cc"],
                    BC = _config["Email:Bc"],
                    Subject = string.Format(_config["Email:Subject"], notify.UploaderApplicationNo),
                    IsBodyHtml = false,
                    MailContent = notify.FileExtracted ? string.Format(_config["Email:BodySuccess"], notify.FolderName, notify.UploaderName, notify.UploaderApplicationNo) : string.Format(_config["Email:BodyFailure"])
                });

                if (sent)
                    LogHandler.WriteLog("Email sent to Admin.", LogEventLevel.Information, _config);
                else
                    LogHandler.WriteLog("Email not sent to Admin.", LogEventLevel.Warning, _config);
            }
            catch (Exception ex)
            {
                LogHandler.WriteLog(ex.Message, LogEventLevel.Fatal, _config);
            }
        }

        public bool SendEmail(MailModel mail)
        {
            char[] separator = { ',' };

            var res = false;
            try
            {
                var host = _config["SmtpServer:Host"];
                var port = _config.GetValue<int>("SmtpServer:Port");

                var client = new SmtpClient(host, port);

                var msg = new MailMessage
                {
                    From = new MailAddress(mail.From)
                };

                //add to
                if (!string.IsNullOrEmpty(mail.To))
                {
                    foreach (var toAddress in mail.To.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        msg.To.Add(new MailAddress(toAddress));
                    }
                }

                //add cc
                if (!string.IsNullOrEmpty(mail.CC))
                {
                    foreach (var ccAddress in mail.CC.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        msg.CC.Add(new MailAddress(ccAddress));
                    }
                }

                //add bcc
                if (!string.IsNullOrEmpty(mail.BC))
                {
                    foreach (var bcAddress in mail.BC.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        msg.Bcc.Add(new MailAddress(bcAddress));
                    }
                }

                //add subject, header, and body
                msg.Subject = mail.Subject;
                msg.IsBodyHtml = mail.IsBodyHtml;
                msg.Body = mail.MailContent;

                client.Send(msg);
                res = true;
            }
            catch (Exception ex)
            {
                LogHandler.WriteLog(ex.Message, LogEventLevel.Fatal, _config);
            }

            return res;

        }

    }
}
