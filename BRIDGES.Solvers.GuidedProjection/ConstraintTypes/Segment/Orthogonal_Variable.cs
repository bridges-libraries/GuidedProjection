using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Segment
{
    /// <summary>
    /// Constraint enforcing a segment to be orthogonal to a variable direction. The list of variables for this constraint consists of:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>P<sub>s</sub></term>
    ///         <description> Variable representing the start point of the segment.</description>
    ///     </item>
    ///     <item> 
    ///         <term>P<sub>e</sub></term>
    ///         <description> Variable representing the end point of the segment.</description>
    ///     </item>
    ///     <item> 
    ///         <term>V</term>
    ///         <description> Variable representing the direction to which the segment must be orthogonal.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <remarks> If the target direction is fixed, refer to the similar constraint <see cref="Orthogonal_Fixed"/>. </remarks>
    public class Orthogonal_Variable : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Orthogonal_Variable"/> class.
        /// </summary>
        /// <param name="dimension"> Dimension of the variables representing the start and the end segment. </param>
        public Orthogonal_Variable(int dimension)
        {
            //     -----     Define LocalHi     -----     //

            int[] columnPointers = new int[3 * dimension + 1];
            int[] rowIndices = new int[(4 * dimension)];
            double[] values = new double[(4 * dimension)];

            columnPointers[0] = 0;
            for (int i_C = 0; i_C < dimension; i_C++)
            {
                columnPointers[i_C + 1] = i_C + 1;
                rowIndices[i_C] = 2 * dimension + i_C;
                values[i_C] = -1;

                columnPointers[dimension + i_C + 1] = dimension + i_C + 1;
                rowIndices[dimension + i_C] = 2 * dimension + i_C;
                values[dimension + i_C] = 1;

                columnPointers[2 * dimension + i_C + 1] = 2 * (dimension + i_C + 1);
                rowIndices[2 * (dimension + i_C)] = i_C ; rowIndices[2 * (dimension + i_C) + 1] = dimension + i_C;
                values[2 * (dimension + i_C)] = -1; values[2 * (dimension + i_C) + 1] = 1;
            }

            List<int> list_RowIndices = new List<int>(rowIndices);
            List<double> list_Values = new List<double>(values);

            Mat.SparseStorages.CompressedColumn<double> storage =
                new Mat.SparseStorages.CompressedColumn<double>(3 * dimension, 3 * dimension, ref list_Values, ref list_RowIndices, ref columnPointers);

            LocalHi = new Mat.SparseMatrix<double>(storage);


            //     -----     Define LocalBi     -----     //

            LocalBi = null;


            //     -----     Define Ci     -----     //

            Ci = 0d;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => "Segment - Orthogonal - Variable Direction";

        #endregion
    }
}
