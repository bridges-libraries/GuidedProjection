using System;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection.Abstracts
{
    /// <summary>
    /// Abstract class providing a basic implementation for an <see cref="IEnergyType"/>
    /// </summary>
    public abstract class EnergyType : IEnergyType
    {
        #region Properties

        /// <summary>
        /// Gets the local vector Ki of the energy.
        /// </summary>
        public Vect.SparseVector<double> LocalKi { get; init; }

        /// <summary>
        /// Gets the scalar value Si of the constraint.
        /// </summary>
        public double Si { get; init; }

        #endregion
    }
}
