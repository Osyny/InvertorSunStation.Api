namespace SunBattery.Core.Helpers.SelectList
{
    public class SelectListItemBase<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }

        public SelectListItemBase() { }

        public SelectListItemBase(T id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
