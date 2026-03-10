using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Segment
{
    /// <summary>
    /// Constraint enforcing a segment to equal a fixed target length. The list of variables for this constraint consists of:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>P<sub>s</sub></term>
    ///         <description> Variable representing the start point of the segment.</description>
    ///     </item>
    ///     <item> 
    ///         <term>P<sub>e</sub></term>
    ///         <description> Variable representing the end point of the segment.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <remarks> If the target length is a variable, refer to the constraint <see cref="Length_Variable"/>. </remarks>
    public class Length_Fixed : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Length_Fixed"/> class.
        /// </summary>
        /// <param name="dimension"> Dimension of the variables representing the start and the end segment. </param>
        /// <param name="length"> Value of the target lenght. </param>
        public Length_Fixed(double length, int dimension)
        {
            //     -----     Define LocalHi     -----     //

            int[] columnPointers = new int[2 * dimension + 1];
            int[] rowIndices = new int[(4 * dimension)];
            double[] values = new double[(4 * dimension)];

            columnPointers[0] = 0;
            for (int i_C = 0; i_C < dimension; i_C++)
            {
                columnPointers[i_C + 1] = 2 * (i_C + 1);
                rowIndices[2 * i_C] = i_C ; rowIndices[2 * i_C + 1] = dimension + i_C;
                values[2 * i_C] = 2d ; values[2 * i_C + 1] = -2d ;

                columnPointers[dimension + i_C + 1] = 2 * (dimension + i_C + 1);
                rowIndices[2 * (dimension + i_C)] = i_C; rowIndices[2 * (dimension + i_C) + 1] = dimension + i_C;
                values[2 * (dimension + i_C)] = -2d; values[2 * (dimension + i_C) + 1] = 2d;
            }

            List<int> list_RowIndices = new List<int>(rowIndices);
            List<double> list_Values = new List<double>(values);

            Mat.SparseStorages.CompressedColumn<double> storage =
                new Mat.SparseStorages.CompressedColumn<double>(2 * dimension, 2 * dimension, ref list_Values, ref list_RowIndices, ref columnPointers);

            LocalHi = new Mat.SparseMatrix<double>(storage);


            //     -----     Define LocalBi     -----     //

            LocalBi = null;


            //     -----     Define Ci     -----     //

            Ci = - (length * length);
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => "Segment - Length - Fixed Scalar";

        #endregion
    }
}
