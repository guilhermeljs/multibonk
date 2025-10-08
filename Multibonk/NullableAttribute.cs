namespace System.Runtime.CompilerServices
{

    /// <summary>
    /// Workaround for missing System.Runtime.CompilerServices.NullableAttribute constructor: the compiler emits an error because 
    /// it requires the NullableAttribute constructor to support nullable reference type metadata, which is not present in this environment.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class NullableAttribute : Attribute
    {
        public NullableAttribute(byte b) { }
        public NullableAttribute(byte[] b) { }
    }
}