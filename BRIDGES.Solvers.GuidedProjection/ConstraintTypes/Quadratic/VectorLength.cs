using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes
{
    /// <summary>
    /// Constraint enforcing a vector variable to equal a fixed target length (computed with euclidean norm). The list of variables of this constraint consists in:
    /// <list type="bullet">
    ///     <item> 
    ///         <term>V</term>
    ///         <description> Variable representing the vector to resize. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public class VectorLength : IConstraintType
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
        /// Initialises a new instance of the <see cref="VectorLength"/> class.
        /// </summary>
        /// <param name="length"> Target length of the vector. </param>
        /// <param name="dimension"> Dimension of the vector variable. </param>
        public VectorLength(double length, int dimension = 3)
        {
            //     -----     Define LocalHi     -----     //

            int[] columnPointers = new int[dimension + 1];
            int[] rowIndices = new int[dimension];
            double[] values = new double[dimension];

            columnPointers[0] = 0;
            for (int i = 0; i < dimension; i++)
            {
                columnPointers[i + 1] = i + 1;
                rowIndices[i] = i;
                values[i] = 2.0;
            }

            List<int> list_RowIndices = new List<int>(rowIndices);
            List<double> list_Values = new List<double>(values);

            Mat.SparseStorages.CompressedColumn<double> storage =
                new Mat.SparseStorages.CompressedColumn<double>(dimension, dimension, ref list_Values, ref list_RowIndices, ref columnPointers);

            LocalHi = new Mat.SparseMatrix<double>(storage);


            //     -----     Define LocalBi     -----     //

            LocalBi = null;


            //     -----     Define Ci     -----     //

            Ci = - (length * length);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new object that is a deep copy of the current instance.
        /// </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public object Clone()
        {
            return new VectorLength(Math.Sqrt(-Ci), LocalHi!.RowCount);
        }


        /// <inheritdoc cref="Object.ToString()"/>
        public override string ToString() => "Vector Length";

        #endregion
    }
}
