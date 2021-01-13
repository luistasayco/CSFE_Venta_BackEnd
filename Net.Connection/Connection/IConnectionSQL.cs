using System.Collections.Generic;

namespace Net.Connection
{
    public interface IConnectionSQL
    {
        public string DevuelveConnectionSQL();
        public void ExecuteSqlNonQuery(string comandSql);
        public void ExecuteSqlNonQueryAuto(string procedureName, object parameters);
        public object ExecuteSqlInsert<T>(string procedureName, T parameters);
        public object ExecuteSqlUpdate<T>(string procedureName, T parameters);
        public object ExecuteSqlDelete<T>(string procedureName, T parameters);
        public DbParametro[] ExecuteSqlNonQuery(string procedureName, DbParametro[] parameters);
        public T ExecuteSqlViewId<T>(string procedureName, T parameters);
        public IEnumerable<T> ExecuteSqlViewFindByCondition<T>(string procedureName, object parameters);
        public IEnumerable<T> ExecuteSqlViewAll<T>(string procedureName, T parameters);
        public IEnumerable<T> ExecuteSqlQuery<T>(string comandSql);
        public IEnumerable<T> ExecuteSqlQuery<T>(string procedureName, DbParametro[] parameters);

    }
}
