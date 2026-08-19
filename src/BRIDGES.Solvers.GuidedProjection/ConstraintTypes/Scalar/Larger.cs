using System;
using System.Collections.Generic;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;
using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Scalar
{
    /// <summary>
    /// Constraint enforcing a scalar variable to be larger than a fixed lower bound. The list of variables of this constraint consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>S</term>
    ///         <description> Scalar variable to maintain above the fixed lower bound. </description>
    ///     </item>
    ///     <item> 
    ///         <term>X</term>
    ///         <description> Dummy scalar variable used for the constraint formulation. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class Larger : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Larger"/> class.
        /// </summary>
        /// <param name="bound"> Value of the lower bound. </param>
        public Larger(double bound)
        {
            //     -----     Define LocalHi     -----     //

            int[] columnPointers = new int[] { 0, 0, 1 };
            List<int> rowIndices = new List<int> { 1 };
            List<double> values = new List<double> { 2d };

            Mat.SparseStorages.CompressedColumn<double> storage = new Mat.SparseStorages.CompressedColumn<double>(2, 2, ref values, ref rowIndices, ref columnPointers);

            LocalHi = new Mat.SparseMatrix<double>(storage);


            //     -----     Define LocalBi     -----     //

            int[] bi_Indices = new int[1] { 0 };
            double[] bi_Values = new double[1] { -1.0 };

            LocalBi = new Vect.SparseVector<double>(2, bi_Indices, bi_Values);


            //     -----     Define Ci     -----     //

            Ci = bound;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="Object.ToString()"/>
        public override string ToString() => "Lower Bound";

        #endregion
    }
}
