
using Station.Core.Enums;
using Station.Web.Host.Extentions;



namespace Station.Core.Helpers.SelectList
{
    public static class ItemsHelpers
    {
        public static List<SelectListItem> GetRoleItems()
        {
            var rolesSelectList = new List<RoleEnum>
            {
                RoleEnum.Admin,
                RoleEnum.User,
            };
            return rolesSelectList.Select(a => a.ToSelectListItem()).OrderByDescending(i => i.Name).ToList();
        }

    }
}
