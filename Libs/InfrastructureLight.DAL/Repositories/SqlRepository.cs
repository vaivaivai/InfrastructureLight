﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NLog;

namespace InfrastructureLight.DAL.Repositories
{
    using Factory;

    public abstract class SqlRepository : ISqlRepository
    {
        private readonly IConnectionManager _connectionFactory;
        private readonly ILogger _logger;

        protected SqlRepository(IConnectionManager connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        #region Members ISqlRepository

        public List<TEntity> Get<TEntity>(string query, Action<TEntity, IDataRecord> fill) where TEntity : new()
        {
            if (string.IsNullOrEmpty(query)) { throw new ArgumentNullException(); }

            var result = new List<TEntity>();
            try
            {
                using (var cn = (SqlConnection)_connectionFactory.CreateConnection())
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = query;
                        cmd.Connection = cn;
                        cn.Open();

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var item = new TEntity();
                                fill(item, dr);
                                result.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        #endregion
    }
}
