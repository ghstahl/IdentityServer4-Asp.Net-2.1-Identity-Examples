using System;
using AspNetCore.Identity.Neo4j.Internal;

namespace AspNetCore.Identity.Neo4j
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class Neo4jLabelAttribute : Attribute
    {
        private readonly string _labelName;

        public Neo4jLabelAttribute(string labelName)
        {
            _labelName = labelName;
        }

        public string LabelName => _labelName;
    }
}