namespace CAMCMSServer.Database.Repository;

public interface IRepository<T> where T : class
{
    #region Methods

    Task<IEnumerable<T>> GetAll();

    Task<T?> GetById(int id, int userId);

    Task<T> Create(T element);

    Task<T> Update(T updatedElement);

    Task Delete(T element);


    #endregion
}