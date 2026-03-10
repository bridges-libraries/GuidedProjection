using System;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;


namespace BRIDGES.Solvers.GuidedProjection.Interfaces
{
    /// <summary>
    /// Interface defining an energy type.
    /// </summary>
    public interface IEnergyType
    {
        #region Properties

        /// <summary>
        /// Gets the local vector Ki of this energy.
        /// </summary>
        public Vect.SparseVector<double> LocalKi { get; }

        /// <summary>
        /// Gets the scalar value Si of this energy.
        /// </summary>
        public double Si { get; }

        #endregion
    }
}