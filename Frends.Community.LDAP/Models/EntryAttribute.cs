using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.LDAP.Models
{
    public class EntryAttribute
    {
        /// <summary>
        /// Type of the AD user attribute.
        /// </summary>
        public AdUserAttribute Attribute { get; set; }

        /// <summary>
        /// Custom attribute name. Use this if the attribute is not available on the Attribute-list.
        /// </summary>
        public string CustomAttributeName { get; set; }

        /// <summary>
        /// Value of the AD user attribute.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Data type of the AD user attribute
        /// </summary>
        [DefaultValue(AttributeType.String)]
        public AttributeType DataType { get; set; }
    }

    public enum AttributeType
    {
        String = 0,
        Int = 1,
        Boolean = 3
    }

    public class ADFlag
    {
        /// <summary>
        /// AD Flag type
        /// </summary>
        public ADFlagType FlagType { get; set; }

        /// <summary>
        /// Flag value
        /// </summary>
        public bool Value { get; set; }
    }
}
