using System;
using System.Collections.Generic;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection
{
    /// <summary>
    /// Constraint for the <see cref="Solver"/>.
    /// </summary>
    public class Constraint
    {
        #region Fields

        /// <summary>
        /// Variables composing the local vector X<sub>local</sub> on which the <see cref="ConstraintType"/> is defined.
        /// </summary>
        private readonly IVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cumulated size of the <see cref="Variables"/>
        /// </summary>
        /// <remarks> It corresponds to the size of the square matrix <see cref="IConstraintType.LocalHi"/> and of the column vector <see cref="IConstraintType.LocalBi"/>. </remarks>
        public int Size { get; init; }


        /// /// <summary>
        /// Gets the constraint type defining the local constraint members
        /// <list type="table">
        ///     <item>
        ///         <term> <see cref="IConstraintType.LocalHi"/> </term>
        ///         <description> Square matrix whose size is defined by the cumulated size of the <see cref="Variables"/>. </description>
        ///     </item>
        ///     <item>
        ///         <term> <see cref="IConstraintType.LocalBi"/> </term>
        ///         <description> Column vector whose size is defined by the cumulated size of the <see cref="Variables"/>. </description>
        ///     </item>
        ///     <item>
        ///         <term> <see cref="IConstraintType.Ci"/> </term>
        ///         <description> Scalar value. </description>
        ///     </item>
        /// </list>
        /// </summary>
        public IConstraintType ConstraintType { get; private set; }

        /// <summary>
        /// Gets the variables composing the local vector X<sub>local</sub>.
        /// </summary>
        public IReadOnlyList<IVariable> Variables => _variables;


        /// <summary>
        /// Gets or sets the weight of this constraint.
        /// </summary>
        public double Weight { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Constraint"/> class.
        /// </summary>
        /// <param name="constraintType"> Constraint type defining the local quantities of this constraint. </param>
        /// <param name="variables"> Variables composing the local vector localX on which the local symmetric matrix Hi and the local vector Bi are defined.</param>
        /// <param name="weight"> Weight of this constraint. </param>
        /// <exception cref="ArgumentException"> The size of the the square matrix of <see cref="ConstraintType"/> must be equal to the cumulated size of the <see cref="Variables"/>. </exception>
        /// <exception cref="ArgumentException"> The size of the the column vector of <see cref="ConstraintType"/> must be equal to the cumulated size of the <see cref="Variables"/>. </exception>
        public Constraint(IConstraintType constraintType, IReadOnlyList<IVariable> variables, double weight)
        {
            Weight = weight;
            ConstraintType = constraintType;

            Size = 0;
            _variables = new IVariable[variables.Count];
            for (int i = 0; i < variables.Count; i++)
            {
                _variables[i] = variables[i];
                Size += variables[i].Size;
            }

            //     -----     Verifications

            if (ConstraintType.LocalHi is null & ConstraintType.LocalBi is null)
            {
                throw new ArgumentException($"The square matrix {nameof(ConstraintType.LocalHi)} and the column vector {nameof(ConstraintType.LocalBi)} cannot be both null."); 
            }

            if (ConstraintType.LocalHi is not null && (ConstraintType.LocalHi.RowCount != Size | ConstraintType.LocalHi.ColumnCount != Size))
            {
                throw new ArgumentException($"The size of the square matrix {nameof(ConstraintType.LocalHi)} must be equal to the cumulated size of the {nameof(Variables)}");
            }
            if(ConstraintType.LocalBi is not null && ConstraintType.LocalBi.Size != Size)
            {
                throw new ArgumentException($"The size of the column vector {nameof(ConstraintType.LocalBi)} must be equal to the cumulated size of the {nameof(Variables)}");
            }

        }

        #endregion
    }
}
