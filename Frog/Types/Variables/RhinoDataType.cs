using System;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Available data type for the grasshopper variable.
    /// </summary>
    public enum VariableDataType : int
    {
        /// <summary>
        /// Variable containing of a single coordinate.
        /// </summary>
        Scalar = 1,
        /// <summary>
        /// Variable containing several coordinates.
        /// </summary>
        Array = 2,
        /// <summary>
        /// Variable representing a three-dimensional point.
        /// </summary>
        Point = 10,
    }
}
