using System.Reflection;
using WeShare.PropertyWrapper;

namespace FL.DbBulk
{
    public class EntityInfo
    {
        public PropertyInfo PropertyInfo { get; set; }

        public string FieldName { get; set; }

        public IGetValue GetMethod { get; set; }
        public object Get(object obj)
        {
            return this.GetMethod.Get(obj);
        }
    }
}