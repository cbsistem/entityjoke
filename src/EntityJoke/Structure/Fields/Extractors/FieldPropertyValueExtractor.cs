﻿using EntityJoke.Structure.Fields;
using System.Reflection;

namespace EntityJoke.Structure.Fields
{
    public class FieldPropertyValueExtractor : FieldValueExtractor
    {
        public FieldPropertyValueExtractor(object objectValue, Field idField) 
            : base(objectValue, idField){}

        public override object Extract()
        {
            PropertyInfo value = ObjectValue().GetType().GetProperty(NameField());
            return value.GetValue(ObjectValue());
        }
    }
}