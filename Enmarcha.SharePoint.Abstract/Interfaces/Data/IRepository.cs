using System.Collections.Generic;

namespace Enmarcha.SharePoint.Abstract.Interfaces.Data
{
    public interface IRepository<T> : IPageable
    {
        T Get(int id);
        ICollection<T> GetAll();
        ICollection<T> GetAll(int page);
        ICollection<T> Query(IQuery query, int page);

        ICollection<T> Query(string query, int page);
        int Insert(T data);
        bool Save(int id, T data);
        bool Delete(int id);
    }
}
