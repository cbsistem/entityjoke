﻿using EntityJoke.Core;
using EntityJoke.Structure.Entities;
using EntityJoke.Structure.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityJoke.Process.Commands
{
    public class InsertCommandGenerator : ICommandSQLGenerator
    {
        readonly Entity entity;
        readonly object objectInsert;

        public InsertCommandGenerator(object objectInsert)
        {
            this.objectInsert = objectInsert;
            this.entity = DictionaryEntitiesMap.INSTANCE.GetEntity(objectInsert.GetType());
        }

        public string Generate()
        {
            return GetInsertThisObject();
        }

        private string GetInsertThisObject()
        {
            return $"{GetInsert()} {GetColumns()} VALUES {GetValues()} RETURNING ID";
        }

        private string GetInsert()
        {
            return $"INSERT INTO {entity.Name}";
        }

        private string GetColumns()
        {
            var builder = new StringBuilder();

            foreach (Field field in GetFieldsOrdered())
                builder.Append(GetColumnName(field));

            return $"({builder.ToString().Substring(2)})";
        }

        private string GetColumnName(Field field)
        {
            var columnName = $", {field.ColumnName}";

            if(!field.IsEntity)
                return columnName;

            return GetJoinIdValue(field) == null ? "" : columnName;
        }

        private IEnumerable<Field> GetFieldsOrdered()
        {
            return entity.GetFields()
                .Where(f => !f.IsKey)
                .OrderBy(f => f.Name);
        }

        private string GetValues()
        {
            var builder = new StringBuilder();

            foreach (Field field in GetFieldsOrdered())
                builder.Append(AddValueInsert(field));

            return $"({builder.ToString().Substring(2)})";
        }

        private string AddValueInsert(Field field)
        {
            var value = GetValueToInsert(field);

            return value == null ? "" : $", {value}";
        }

        private string GetValueToInsert(Field field)
        {
            if(!field.IsEntity)
                return new FieldValueFormater(objectInsert, field).Format();

            return GetJoinIdValue(field);
        }

        private string GetJoinIdValue(Field field)
        {
            var join = field.GetExtractor(objectInsert).Extract();

            if (join == null)
                return null;

            var entityJoin = DictionaryEntitiesMap.INSTANCE.GetEntity(join.GetType());
            var idField = entityJoin.FieldDictionary["id"];

            return new FieldValueFormater(join, idField).Format();
        }

    }
}
