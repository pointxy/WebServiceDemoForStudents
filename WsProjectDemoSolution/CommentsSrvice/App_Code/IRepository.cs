using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentsSrvice.App_Code
{
    public interface IRepository<T>
    {
        T GetById(int id);
        int AddNew(T entity);
        int Update(T entity);
        int Delete(int id);
        List<T> GetList();
    }
}