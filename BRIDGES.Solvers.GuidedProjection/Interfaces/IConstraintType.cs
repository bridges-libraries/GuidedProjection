using System;

using BRIDGES.Numerics.LinearAlgebra.Vectors;
using BRIDGES.Numerics.LinearAlgebra.Matrices;


namespace BRIDGES.Solvers.GuidedProjection.Interfaces
{
    /// <summary>
    /// Abstract class defining a constraint type.
    /// </summary>
    public interface IConstraintType : ICloneable
    {
        #region Properties

        /// <summary>
        /// Gets the local symmetric matrix Hi of the constraint.
        /// </summary>
        public SparseMatrix<double>? LocalHi { get; }

        /// <summary>
        /// Gets the local vector Bi of the constraint.
        /// </summary>
        public SparseVector<double>? LocalBi { get; }

        /// <summary>
        /// Gets the scalar value Ci of the constraint.
        /// </summary>
        public double Ci { get; }

        #endregion
    }
}
