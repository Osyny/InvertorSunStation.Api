namespace SunBattery_Api.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string GetEmailVerificationTemplate(string link, string firstName, string password)
        {

            return $"{firstName}, Thank you for registering for your free online memorial website at 'Invertor portal'.Please confirm your account by '{link}' clicking here. Password is {password}";
        }
    }
}
