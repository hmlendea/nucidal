using System;

namespace NuciDAL.IO
{
    /// <summary>
    /// Specifies the column order for a property when serialising to CSV.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CsvColumnAttribute"/> class.
    /// </remarks>
    /// <param name="order">The column order (lower values appear first).</param>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CsvColumnAttribute(int order) : Attribute
    {
        /// <summary>
        /// Gets the column order.
        /// </summary>
        /// <value>The column order.</value>
        public int Order { get; } = order;
    }
}