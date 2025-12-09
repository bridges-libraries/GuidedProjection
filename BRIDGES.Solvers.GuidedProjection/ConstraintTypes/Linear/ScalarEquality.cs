using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Interfaces;
using System.Security.Cryptography.X509Certificates;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes
{
    /// <summary>
    /// Constraint enforcing a numeric variable to be equal to a fixed value. The list of variables of this constraint consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>V</term>
    ///         <description> Scalar variable. which should match the specified fixed value. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class ScalarEquality : IConstraintType
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

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ScalarEquality"/> class.
        /// </summary>
        /// <param name="value"> Target value for the numeric variable. </param>
        public ScalarEquality(double value)
        {
            //     -----     Define LocalHi     -----     //

            LocalHi = null;


            //     -----     Define LocalBi     -----     //

            int[] rowIndices = new int[1] { 0 };
            double[] values = new double[1] { -1d };

            LocalBi = new Vect.SparseVector<double>(1, rowIndices, values);


            //     -----     Define Ci     -----     //

            Ci = value;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new object that is a deep copy of the current instance.
        /// </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public object Clone()
        {
            return new ScalarEquality(Ci);
        }


        /// <inheritdoc cref="Object.ToString()"/>
        public override string ToString() => "Scalar Equality";

        #endregion
    }
}
