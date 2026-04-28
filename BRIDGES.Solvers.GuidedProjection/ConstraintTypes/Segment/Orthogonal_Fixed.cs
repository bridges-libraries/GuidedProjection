using System;
using System.Collections.Generic;

using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection.ConstraintTypes.Segment
{
    /// <summary>
    /// Constraint enforcing a segment to be orthogonal to a fixed direction <em>V</em>. The list of variables of this constraint consists in:
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
    /// <remarks> If the target direction is a variable, refer to the similar constraint <see cref="Orthogonal_Variable"/>. </remarks>
    public class Orthogonal_Fixed : ConstraintType
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Orthogonal_Fixed"/> class.
        /// </summary>
        /// <param name="direction"> Coordinates of the direction vector to which the segment must be orthogonal. </param>
        public Orthogonal_Fixed(double[] direction)
        {
            // ----- Unitise the direction ----- //

            bool isZero = true;
            double length = 0d;
            for (int i = 0; i < direction.Length; i++)
            {
                if (isZero & Helpers.Settings.AbsolutePrecision < Math.Abs(direction[i])) { isZero = false; }

                length += direction[i] * direction[i];
            }
            length = Math.Sqrt(length);

            if (isZero) { throw new DivideByZeroException("The length of the target direction must be different from zero."); }

            for (int i = 0; i < direction.Length; i++)
            {
                direction[i] = direction[i] / length;
            }


            //     -----     Define LocalHi     -----     //

            LocalHi = null;


            //     -----     Define LocalBi     -----     //

            int capacity = 2 * direction.Length;

            List<int> indices = new List<int>(capacity);
            List<double> values = new List<double>(capacity);


            Dictionary<int, double> coordinates = new Dictionary<int, double>(2 * direction.Length);
            for (int i = 0; i < direction.Length; i++)
            {
                indices.Add(i); values.Add(-direction[i]);
                indices.Add(direction.Length + i); values.Add(direction[i]);
            }

            LocalBi = new Vect.SparseVector<double>(2 * direction.Length, indices, values);


            //     -----     Define Ci     -----     //

            Ci = 0d;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() => "Segment - Orthogonal - Fixed Direction";

        #endregion
    }
}
