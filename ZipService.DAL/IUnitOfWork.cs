using LanguageExt;
using LanguageExt.Common;
using ZipService.Domain;

namespace ZipService.DAL
{
    public interface IUnitOfWork<T> : IDisposable where T : BaseEntity
    {
        Result<Guid> Add(T entity);
        Result<Unit> Update(T entity);
        Result<Unit> Delete(Guid id);
        Result<Unit> Delete(T entity);
        Option<T> GetById(Guid id);
        Result<IEnumerable<T>> List();
        Result<Unit> SaveChanges();
    }
}