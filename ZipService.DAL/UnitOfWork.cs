using LanguageExt;
using LanguageExt.Common;
using ZipService.Domain;

namespace ZipService.DAL
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : BaseEntity
    {
        // No actual DB connection for now needed, just a mock.
        private static List<T> _entities = new List<T>();
        private static readonly object _entitiesLock = new object();

        public Result<Guid> Add(T entity)
        {
            lock (_entitiesLock)
            {
                _entities.Add(entity);
            }

            return new Result<Guid>(entity.Id);
        }

        public Result<Unit> Delete(T entity)
        {
            lock (_entitiesLock)
            {
                if (!_entities.Contains(entity))
                {
                    return new Result<Unit>(new Exception($"No {typeof(T)} entity with id: {entity.Id}"));
                }

                _entities.Remove(entity);
            }

            return new Result<Unit>(new Unit());
        }

        public Result<Unit> Delete(Guid id)
        {
            T? entity;

            lock (_entitiesLock)
            {
                entity = _entities.SingleOrDefault(x => x.Id == id);
            }

            if (entity == null)
            {
                return new Result<Unit>(new Exception($"No {typeof(T)} entity with id: {id}"));
            }

            return Delete(entity);
        }

        public Option<T> GetById(Guid id)
        {
            T? entity;

            lock (_entitiesLock)
            {
                entity = _entities.SingleOrDefault(x => x.Id == id);
            }

            if (entity == null)
            {
                return Option<T>.None;
            }

            return entity;
        }

        public Result<IEnumerable<T>> List()
        {
            lock (_entitiesLock)
            {
                return _entities.ToArray();
            }
        }

        public Result<Unit> SaveChanges()
        {
            // No need for that atm.
            throw new NotImplementedException();
        }

        public Result<Unit> Update(T entity)
        {
            // No need for that atm.
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
