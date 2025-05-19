using System.Collections.Specialized;

namespace SunBattery.Core.Helpers.SelectList
{
    public class SelectListItem : SelectListItemBase<int>
    {
        public SelectListItem()
        {

        }

        public SelectListItem(int id, string name) : base(id, name)
        { }
    }
}
