using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection.Abstracts
{
    /// <summary>
    /// Abstract class providing a basic implementation for an <see cref="IConstraintType"/> which can be updated at each iteration.
    /// </summary>
    public abstract class UpdatedConstraintType : IConstraintType
    {
        #region Properties

        /// <summary>
        /// Gets the local symmetric matrix Hi of the constraint.
        /// </summary>
        public Mat.SparseMatrix<double>? LocalHi { get; internal protected set; }

        /// <summary>
        /// Gets the local vector Bi of the constraint.
        /// </summary>
        public Vect.SparseVector<double>? LocalBi { get; internal protected set; }

        /// <summary>
        /// Gets the scalar value Ci of the constraint.
        /// </summary>
        public double Ci { get; internal protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the members (i.e. LocalHi and LocalBi and Ci) of the constraint using auxiliary variables or auxiliary objects.
        /// </summary>
        /// <param name="variables"> Variables composing the local vector localX on which the local symmetric matrix Hi and the local vector Bi are defined. </param>
        public abstract void ComputeMembers(IReadOnlyList<IVariable> variables, out Mat.SparseMatrix<double>? localHi, out Vect.SparseVector<double>? localBi, out double ci);

        #endregion
    }
}
