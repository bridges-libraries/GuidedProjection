using System;
using System.Collections.Generic;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;
using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes
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
    public class LowerBound : IConstraintType
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
        /// Initialises a new instance of the <see cref="LowerBound"/> class.
        /// </summary>
        /// <param name="bound"> Value of the lower bound. </param>
        public LowerBound(double bound)
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

        #region Public Methods

        /// <summary>
        /// Creates a new object that is a deep copy of the current instance.
        /// </summary>
        /// <returns> A new object that is a copy of this instance. </returns>
        public object Clone()
        {
            return new LowerBound(Ci);
        }


        /// <inheritdoc cref="Object.ToString()"/>
        public override string ToString() => "Lower Bound";

        #endregion
    }
}
