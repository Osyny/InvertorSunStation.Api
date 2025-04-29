using SunBattery_Api.Models.EmailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Management.Service.Services
{
    public interface IEmailService
    {
        string SendEmail(Message message);
    }
}
