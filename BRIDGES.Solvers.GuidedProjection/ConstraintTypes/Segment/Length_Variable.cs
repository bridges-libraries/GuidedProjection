using System;
using System.Collections.Generic;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;
using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Segment
{
    /// <summary>
    /// Constraint enforcing a scalar variable to match with the distance between two variables. The list of variables of this constraint consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>P<sub>i</sub></term>
    ///         <description> First variable with which the distance is being computed. </description>
    ///     </item>
    ///     <item> 
    ///         <term>P<sub>j</sub></term>
    ///         <description> Second variable with which the distance is being computed. </description>
    ///     </item>
    ///     <item> 
    ///         <term>L</term>
    ///         <description> Scalar variable to match with the distance. </description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <remarks> If the target length is fixed, refer to the constraint <see cref="Length_Fixed"/>. </remarks>
    public class Length_Variable : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Length_Variable"/> class.
        /// </summary>
        /// <param name="dimension"> Common dimension of the variables with which the distance is being computed. </param>
        public Length_Variable(int dimension)
        {
            //     -----     Define LocalHi     -----     //

            int[] columnPointers = new int[2 * dimension + 2];
            int[] rowIndices = new int[4 * dimension + 1];
            double[] values = new double[4 * dimension + 1];

            columnPointers[0] = 0;
            for (int i_C = 0; i_C < dimension; i_C++)
            {
                columnPointers[i_C + 1] = 2 * (i_C + 1);
                rowIndices[2 * i_C] = i_C; rowIndices[2 * i_C + 1] = dimension + i_C;
                values[2 * i_C] = 2.0; values[2 * i_C + 1] = -2.0;

                columnPointers[dimension + i_C + 1] = 2 * (dimension + i_C + 1);
                rowIndices[2 * (dimension + i_C)] = i_C; rowIndices[2 * (dimension + i_C) + 1] = dimension + i_C;
                values[2 * (dimension + i_C)] = -2.0; values[2 * (dimension + i_C) + 1] = 2.0;
            }

            columnPointers[2 * dimension + 1] = 4 * dimension + 1;
            rowIndices[4 * dimension] = 2 * dimension;
            values[4 * dimension] = -2.0;

            List<int> list_RowIndices = new List<int>(rowIndices);
            List<double> list_Values = new List<double>(values);

            Mat.SparseStorages.CompressedColumn<double> storage = 
                new Mat.SparseStorages.CompressedColumn<double>(2 * dimension + 1, 2 * dimension + 1, ref list_Values, ref list_RowIndices, ref columnPointers);

            LocalHi = new Mat.SparseMatrix<double>(storage);


            //     -----     Define LocalBi     -----     //

            LocalBi = null ;


            //     -----     Define Ci     -----     //

            Ci = 0d;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => "Segment - Length - Variable Scalar";

        #endregion
    }
}
