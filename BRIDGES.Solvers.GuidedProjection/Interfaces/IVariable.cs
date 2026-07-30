using System;


namespace BRIDGES.Solvers.GuidedProjection.Interfaces
{
    /// <summary>
    /// Interfaces representing a set of coordinates for the global vector X. 
    /// </summary>
    public interface IVariable
    {
        #region Properties 

        /// <summary>
        /// Gets the coordinate at the specified index in this variable.
        /// </summary>
        /// <param name="index"> The zero-based index of the variable's coordinate. </param>
        /// <returns> The coordinate at the specified index. </returns>
        public double this[int index] { get; }

        /// <summary>
        /// Get the number of coordinate of this variable.
        /// </summary>
        public int Size { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Translate the coordinate's index into the coordinate index of the referenced array.
        /// </summary>
        /// <param name="index"> The zero-based index of the coordinate. </param>
        /// <returns> The zero-based index of the coordinate in the referenced array. </returns>
        public int ReferenceIndex(int index);


        /// <summary>
        /// Creates an array representation of the variable.
        /// </summary>
        /// <returns> The <see langword="double"/>-precision array representing the variable. </returns>
        public double[] ToArray();

        #endregion
    }
}
