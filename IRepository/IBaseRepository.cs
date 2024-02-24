using System.Linq.Expressions;

namespace WebEnterprise.IRepository;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity.BaseEntity
{
    Task<TEntity> FindAsync(string id);

    Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter);

    Task<IList<TEntity>> FindListAsync(Expression<Func<TEntity, bool>> filter);

    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);
}