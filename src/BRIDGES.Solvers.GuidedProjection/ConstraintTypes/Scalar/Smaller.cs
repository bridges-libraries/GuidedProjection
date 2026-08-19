using System;
using System.Collections.Generic;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;
using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Scalar
{
    /// <summary>
    /// Constraint enforcing a scalar variable to be smaller than a fixed upper bound. The list of variables of this constraint consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>S</term>
    ///         <description> Scalar variable to maintain under the fixed upper bound. </description>
    ///     </item>
    ///     <item> 
    ///         <term>X</term>
    ///         <description> Dummy scalar variable used for the constraint formulation. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class Smaller : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Smaller"/> class.
        /// </summary>
        /// <param name="bound"> Value of the upper bound. </param>
        public Smaller(double bound)
        {
            //     -----     Define LocalHi     -----     //

            int[] columnPointers = new int[3] { 0, 0, 1 };
            List<int> rowIndices = new List<int> { 1 };
            List<double> values = new List<double> { -2d };

            Mat.SparseStorages.CompressedColumn<double> storage =
                new Mat.SparseStorages.CompressedColumn<double>(2, 2, ref values, ref rowIndices, ref columnPointers);

            LocalHi = new Mat.SparseMatrix<double>(storage);


            //     -----     Define LocalBi     -----     //

            int[] bi_Indices = new int[] { 0 };
            double[] bi_Values = new double[] { -1.0 };

            LocalBi = new Vect.SparseVector<double>(2, bi_Indices, bi_Values );


            //     -----     Define Ci     -----     //

            Ci = bound;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => "Scalar - Smaller - Fixed Bound";

        #endregion
    }
}
