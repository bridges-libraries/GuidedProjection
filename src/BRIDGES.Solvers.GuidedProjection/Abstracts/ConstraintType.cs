using System;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection.Abstracts
{
    /// <summary>
    /// Abstract class providing a basic implementation for an <see cref="IConstraintType"/>
    /// </summary>
    public abstract class ConstraintType : IConstraintType
    {
        #region Properties

        /// <summary>
        /// Gets the local symmetric matrix Hi of the constraint.
        /// </summary>
        public Mat.SparseMatrix<double>? LocalHi { get; init; }

        /// <summary>
        /// Gets the local vector Bi of the constraint.
        /// </summary>
        public Vect.SparseVector<double>? LocalBi { get; init; }

        /// <summary>
        /// Gets the scalar value Ci of the constraint.
        /// </summary>
        public double Ci { get; init; }

        #endregion
    }
}
