using Microsoft.Extensions.Primitives;

namespace WebEnterprise.Constants;

public static class StringEnums
{
    private class StringValue : Attribute
    {
        public string Value { get; }
        public StringValue(string value)
        {
            Value = value;
        }
    }
    
    public static string ToValue(this Enum thisEnum)
    {
        string output = null;
        var type = thisEnum.GetType();

        var fieldInfo = type.GetField(thisEnum.ToString());
        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs != null && attrs.Length > 0)
            {
                output = attrs[0].Value;
            }
        }

        return output;
    }
    
    public enum CategoryStatus 
    {
        [StringValue("Pending")] Pending,
        [StringValue("Approved")] Approved,
        [StringValue("Rejected")] Rejected,
    }
    
    public enum GenderType
    {
        [StringValue("Male")]
        Male,
        [StringValue("Female")]
        Female,
        [StringValue("Other")]
        Other
    }

}