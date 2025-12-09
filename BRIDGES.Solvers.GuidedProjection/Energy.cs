using System;
using System.Collections.Generic;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection
{
    /// <summary>
    /// Class defining an energy for the <see cref="Solver"/>.
    /// </summary>
    public struct Energy
    {
        #region Fields

        /// <summary>
        /// Variables composing the local vector localX on which the <see cref="IEnergyType"/> is defined.
        /// </summary>
        private readonly IVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cumulated size of the <see cref="Variables"/>
        /// </summary>
        /// <remarks> It corresponds to the size of the column vector <see cref="IEnergyType.LocalKi"/>. </remarks>
        public int Size { get; init; }


        /// <summary>
        /// Gets the energy type defining the local vector <see cref="IEnergyType.LocalKi"/> and the scalar value <see cref="IEnergyType.Si"/>.
        /// </summary>
        public IEnergyType EnergyType { get; private set; }

        /// <summary>
        /// Gets the variables composing the local vector localX.
        /// </summary>
        public IReadOnlyList<IVariable> Variables => _variables;


        /// <summary>
        /// Gets or sets the weight of this energy.
        /// </summary>
        public double Weight { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Energy"/> class.
        /// </summary>
        /// <param name="energyType"> Energy type defining the local quantities of the energy. </param>
        /// <param name="variables"> Variables composing the local vector localX. </param>
        /// <param name="weight"> Weight of the energy. </param>
        /// <exception cref="ArgumentException"> The size of the the column vector of <see cref="EnergyType"/> must be equal to the cumulated size of the <see cref="Variables"/>. </exception>
        public Energy(IEnergyType energyType, IReadOnlyList<IVariable> variables, double weight)
        {
            Weight = weight;
            EnergyType = energyType;

            Size = 0;
            _variables = new IVariable[variables.Count];
            for (int i = 0; i < variables.Count; i++)
            {
                _variables[i] = variables[i];
                Size += variables[i].Size;
            }


            //     -----     Verifications

            if (EnergyType.LocalKi is not null && EnergyType.LocalKi.Size != Size)
            {
                throw new ArgumentException($"The size of the the column vector {nameof(EnergyType.LocalKi)} must be equal to the cumulated size of the {nameof(Variables)}");
            }
        }

        #endregion
    }
}
