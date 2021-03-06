﻿using System.Collections.Generic;

namespace InfrastructureLight.Domain.Comparers
{
    using Interfaces;

    /// <summary>
    ///     Entity Equality Comparer
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity>
        where TEntity : class, IKeyedEntity<int>
    {
        public bool Equals(TEntity x, TEntity y)
        {
            if (x == null || y == null)
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(TEntity obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
