using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Scalar
{
    /// <summary>
    /// Constraint enforcing a vector variable to be equal to a fixed vector. The list of variables of this constraint consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>V</term>
    ///         <description> Vector variable which should match the specified fixed vector. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class Equal : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Equal"/> class for a scalar variable.
        /// </summary>
        /// <param name="value"> Target value for the numeric variable. </param>
        public Equal(double value)
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


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => "Vector - Equal";

        #endregion
    }
}
