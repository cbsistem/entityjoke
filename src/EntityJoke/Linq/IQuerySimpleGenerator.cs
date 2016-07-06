﻿using EntityJoke.Structure.Entities;

namespace EntityJoke.Linq
{
    public interface IQuerySimpleGenerator
    {
        void SetEntity(Entity entity);
        void SetWhere(string where);
        void SetOrderBy(string orderBy);

        string GetSqlCommand();
    }
}
