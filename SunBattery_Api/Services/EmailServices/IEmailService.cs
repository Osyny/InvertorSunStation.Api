
using SunBattery_Api.Models.EmailSenderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunBattery_Api.Services.EmailServices
{
    public interface IEmailService
    {
        string SendEmail(Message message);
    }
}
