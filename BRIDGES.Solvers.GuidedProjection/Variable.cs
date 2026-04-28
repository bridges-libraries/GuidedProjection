using System;
using System.Collections.Generic;

using BRIDGES.Solvers.GuidedProjection.Interfaces;


namespace BRIDGES.Solvers.GuidedProjection
{
    /// <summary>
    /// Variable for the <see cref="Solver"/> which represents a part (i.e consecutive coordinates) of the global vector X.
    /// </summary>
    public sealed class Variable : IVariable, IEquatable<Variable>
    {
        #region Fields

        /// <summary>
        /// Segment of the global vector X representing this variable. 
        /// </summary>
        /// <remarks>
        /// The <see cref="ArraySegment{T}"/> initially refers to an array representing only this variable. <br/>
        /// Once it is provided to the <see cref="Solver"/> and the <see cref="Solver"/> is <see cref="Solver.InitialiseX()"/>,
        /// the <see cref="ArraySegment{T}"/> refers to indices in the global vector <see cref="Solver.X"/>.
        /// </remarks>
        private ArraySegment<double> _coordinates;

        #endregion

        #region Properties

        /// <inheritdoc/>
        /// <remarks> If <see langword="true"/>, the variable was added to a <see cref="Solver"/> and it references the global vector X. </remarks>
        public bool IsReference => !(_coordinates.Count == _coordinates.Array!.Length);


        /// <inheritdoc/>
        /// <exception cref="IndexOutOfRangeException"> The index must be positive and smaller than the size of the variable. </exception>
        public double this[int index] => _coordinates.Array![_coordinates.Offset + index];
        

        /// <inheritdoc/>
        public int Size => _coordinates.Count;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Variable"/> class from its coordinates.
        /// </summary>
        /// <param name="coordinates"> Coordinates of the variable. </param>
        public Variable(params double[] coordinates)
        {
            //     -----     Verifications
            if (coordinates is null) { throw new ArgumentNullException(nameof(coordinates)); }
            if (coordinates.Length == 0) { throw new RankException("The variable must be initialised with at least one coordinate."); }

            double[] array = (double[])coordinates.Clone();

            this._coordinates = new ArraySegment<double>(array);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Variable"/> class from a list of coordinates.
        /// </summary>
        /// <param name="coordinates"> List of coordinates of the variable. </param>
        public Variable(IList<double> coordinates)
        {
            //     -----     Verifications
            if (coordinates is null) { throw new ArgumentNullException(nameof(coordinates)); }
            if (coordinates.Count == 0) { throw new RankException("The variable must be initialised with at least one coordinate."); }

            double[] array = new double[coordinates.Count];
            for (int i = 0; i < array.Length; i++) { array[i] = coordinates[i];  }

            this._coordinates = new ArraySegment<double>(array);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a <see cref="Span{T}"/> representation of the variable
        /// </summary>
        /// <returns> The <see cref="Span{T}"/> representation of the variable. </returns>
        public Span<double> ToSpan()
        {
            return new Span<double>(_coordinates.Array, _coordinates.Offset, _coordinates.Count);
        }

        /// <summary>
        /// Creates a <see cref="Span{T}"/> representation of the variable
        /// </summary>
        /// <param name="start"> Zero-based index of the first coordinate to include in the <see cref="Span{T}"/>. </param>
        /// <param name="count"> Number of coordinate to include in the <see cref="Span{T}"/>. </param>
        /// <returns> The <see cref="Span{T}"/> representation of the variable. </returns>
        public Span<double> ToSpan(int start, int count)
        {
            return new Span<double>(_coordinates.Array, _coordinates.Offset + start, count);
        }


        //     -----     Implement IVariable     -----     //

        /// <inheritdoc/>
        public int ReferenceIndex(int index) => _coordinates.Offset + index;


        /// <inheritdoc/>
        public double[] ToArray()
        {
            double[] array = new double[Size];
            for (int i = 0; i < Size; i++) 
            { 
                array[i] = this[i]; 
            }

            return array;
        }


        //     -----     Implement IEquatable<.>     -----     //

        /// <summary> Indicates whether this <see cref="Variable"/> is equal to another <see cref="Variable"/>. </summary>
        /// <param name="other"> A <see cref="Variable"/> to compare with this one. </param>
        /// <returns> <see langword="true"/> if their <see cref="ArraySegment{T}"/> field are equal, <see langword="false"/> otherwise. </returns>
        public bool Equals(Variable? other) => other is not null && _coordinates.Equals(other._coordinates);

        #endregion

        #region Other Methods

        /// <summary>
        /// Changes the reference of this variable.
        /// </summary>
        /// <remarks> Interfering with the variable's reference can create issues with the solving. </remarks>
        /// <param name="array"> New array to refer to. </param>
        /// <param name="offset"> Zero-based index of the variable's first coordinate in the array. </param>
        internal void ChangeReference(double[] array, int offset)
        {
            _coordinates = new ArraySegment<double>(array, offset, Size);
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="Object.Equals(object)"/>
        public override bool Equals(object? obj)
        {
            return obj is Variable variable && Equals(variable);
        }

        /// <inheritdoc cref="Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            return 1716805434 + _coordinates.GetHashCode();
        }

        #endregion
    }
}
