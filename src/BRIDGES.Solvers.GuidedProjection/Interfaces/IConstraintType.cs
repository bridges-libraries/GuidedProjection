using System;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;


namespace BRIDGES.Solvers.GuidedProjection.Interfaces
{
    /// <summary>
    /// Interface defining a constraint type.
    /// </summary>
    public interface IConstraintType
    {
        #region Properties

        /// <summary>
        /// Gets the local symmetric matrix Hi of the constraint.
        /// </summary>
        public Mat.SparseMatrix<double>? LocalHi { get; }

        /// <summary>
        /// Gets the local vector Bi of the constraint.
        /// </summary>
        public Vect.SparseVector<double>? LocalBi { get; }

        /// <summary>
        /// Gets the scalar value Ci of the constraint.
        /// </summary>
        public double Ci { get; }

        #endregion
    }
}
