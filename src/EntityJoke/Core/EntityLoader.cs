﻿using EntityJoke.Process;
using EntityJoke.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityJoke.Core
{
    public class EntityLoader
    {
        private object obj;

        public int IndexColumn;
        public Entity Entity;
        public DataRow Row;
        public DataColumnCollection Columns;

        public Dictionary<string, object> DictionaryObjectsProcessed;

        public object LoadInstance()
        {
            obj = Activator.CreateInstance(Entity.Type);
            LoadObject();
            return obj;
        }

        private void LoadObject()
        {
            if (IsObjectProcessed())
                RecoverObject();
            else
                CreateObject();
        }

        private bool IsObjectProcessed()
        {
            ProcessField();
            return DictionaryObjectsProcessed.ContainsKey(GetKey(obj));
        }

        private string GetKey(object obj)
        {
            return new KeyDictionaryObjectExtractor(obj).Extract();
        }

        private object GetObjectInDictionary()
        {
            return DictionaryEntitiesObjects.GetInstance().GetObject(obj);
        }

        private void RecoverObject()
        {
            obj = GetObjectInDictionary();
        }

        private void CreateObject()
        {
            ProccesColumns();
            PutObjectInDictionary();
        }

        private void ProccesColumns()
        {
            int limiteLoop = EntityColumnsLength();

            for (; IndexColumn < limiteLoop; IndexColumn++)
                ProcessField();

            Entity.Joins.ForEach(j => ProcessJoin(j));
        }

        private int EntityColumnsLength()
        {
            int fieldsNotEntity = Entity.GetFields().Where(f => !f.IsEntity).ToList().Count;
            return fieldsNotEntity + IndexColumn;
        }

        private void ProcessField()
        {
            DataColumn column = GetCurrentColumn();
            var value = Row[column.ColumnName];

            if (!IsNullValue(value))
                SetFieldValue(GetColumnField(column), value);
        }

        private DataColumn GetCurrentColumn()
        {
            return Columns[IndexColumn];
        }

        private bool IsNullValue(object value)
        {
            return value.GetType() == typeof(DBNull);
        }

        private Field GetColumnField(DataColumn column)
        {
            return Entity.FieldDictionary[GetOriginalName(column)];
        }

        private string GetOriginalName(DataColumn column)
        {
            int indexOf = column.ColumnName.IndexOf("_");
            return column.ColumnName.Substring(indexOf + 1);
        }

        private void SetFieldValue(Field field, object value)
        {
            new FieldValueSetter(obj, field, value).Set();
        }

        private void ProcessJoin(EntityJoin join)
        {
            SetFieldValue(join.Field, GetJoinValue(join));
        }

        private object GetJoinValue(EntityJoin join)
        {
            return new EntityLoaderBuilder()
                .Entity(join.Entity)
                .Row(Row)
                .Columns(Columns)
                .IndexColumn(IndexColumn)
                .Dictionary(DictionaryObjectsProcessed)
                .Build();
        }

        private void PutObjectInDictionary()
        {
            if (IsObjectInDictionaryEntities())
                RefreshObject();
            else
                DictionaryEntitiesObjects.GetInstance().AddOrRefreshObject(obj);

            DictionaryObjectsProcessed.Add(GetKey(obj), obj);
        }

        private bool IsObjectInDictionaryEntities()
        {
            return DictionaryEntitiesObjects.GetInstance().GetObject(obj) != null;
        }

        private void RefreshObject()
        {
            var objectDictionary = DictionaryEntitiesObjects.GetInstance().GetObject(obj);
            Entity.GetFields().ForEach(f => RefreshFieldValue(objectDictionary, f));
            obj = objectDictionary;
        }

        private void RefreshFieldValue(object objectDictionary, Field field)
        {
            new FieldValueSetter(objectDictionary, field, GetValue(field)).Set();
        }

        private object GetValue(Field field)
        {
            return new ValueFieldExtractor(obj, field).Extract();
        }
    }
}
