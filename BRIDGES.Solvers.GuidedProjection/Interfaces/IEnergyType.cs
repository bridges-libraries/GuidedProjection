using System;

using BRIDGES.Numerics.LinearAlgebra.Vectors;


namespace BRIDGES.Solvers.GuidedProjection.Interfaces
{
    /// <summary>
    /// Abstract class defining an energy type.
    /// </summary>
    public interface IEnergyType : ICloneable
    {
        #region Properties

        /// <summary>
        /// Gets the local vector Ki of this energy.
        /// </summary>
        public SparseVector<double> LocalKi { get; }

        /// <summary>
        /// Gets the scalar value Si of this energy.
        /// </summary>
        public double Si { get; }

        #endregion
    }
}
