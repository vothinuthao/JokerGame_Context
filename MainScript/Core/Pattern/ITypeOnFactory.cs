using System;

namespace Core.Pattern
{
    /// <typeparam name="T">group of type variants</typeparam>
    public interface ITypeOnFactory<out T> where T : Enum
    {
        T GetProductType();
    }

    
}