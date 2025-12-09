using BRIDGES.Solvers.GuidedProjection.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace BRIDGES.Solvers.GuidedProjection
{
    /// <summary>
    /// SubVariable for the <see cref="Solver"/>, whose retained components are referenced from a <see cref="Variable"/>.
    /// </summary>
    /// <remarks> <see cref="SubVariable"/> don't need to be add to the <see cref="Solver"/> (as long as the <see cref="Variable"/> is. </remarks>
    public sealed class SubVariable : IVariable, IEquatable<SubVariable>
    {
        #region Fields

        /// <summary>
        /// Indices of the components in the variable which are referenced in the SubVariable.
        /// </summary>
        private readonly int[] _indices;

        #endregion

        #region Properties

        /// <summary>
        /// Variable to which this SubVariable refers for its components.
        /// </summary>
        public Variable ReferenceVariable { get; init; }


        /// <inheritdoc/>
        public bool IsReference => true;


        /// <inheritdoc/>
        public double this[int index] => ReferenceVariable[_indices[index]];

        /// <inheritdoc/>
        public int Size => _indices.Length;


        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="SubVariable"/> class from the reference variable and the retained indices.
        /// </summary>
        /// <param name="variable"> Reference variable. </param>
        /// <param name="indices"> Retained indices of the components in the variable. </param>
        /// <exception cref="IndexOutOfRangeException"> One of the retained index is larger the dimension of the reference array. </exception>
        /// <exception cref="ArgumentException"> Two retained indices can not be equal. </exception>
        public SubVariable(Variable variable, IReadOnlyList<int> indices)
        {
            //     -----     Verifications
            
            ArgumentNullException.ThrowIfNull(variable);


            if ( !(indices.Count < variable.Size) )
            {
                throw new ArgumentException("The number of indices is larger or equal to the size of the variable V.");
            }


            int[] sortedIndices = new int[indices.Count];
            for (int i = 0; i < indices.Count; i++) { sortedIndices[i] = indices[i]; }

            Array.Sort(sortedIndices, (a, b) => a.CompareTo(b));

            if (sortedIndices[0] < 0)
            {
                throw new ArgumentException("An index is negative.");
            }
            for (int i = 0; i < sortedIndices.Length - 1; i++)
            {
                if (sortedIndices[i] == sortedIndices[i + 1])
                {
                    throw new ArgumentException("Two indices are equal.");
                }
            }
            if (sortedIndices[^1] >= variable.Size)
            {
                throw new ArgumentException("An index is larger than the size of the reference variable.");
            }

            //     -----     Initialisations

            this.ReferenceVariable = variable;

            this._indices = sortedIndices;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SubVariable"/> class from the reference variable and a range of indices.
        /// </summary>
        /// <param name="variable"> Reference variable. </param>
        /// <param name="start"> Zero-based index of the first component to retain in the variable. </param>
        /// <param name="count"> Number of component to retain. </param>
        public SubVariable(Variable variable, int start, int count)
        {
            //     -----     Verifications
            if (variable is null) { throw new ArgumentNullException(nameof(variable)); }


            this.ReferenceVariable = variable;

            this._indices = new int[count];
            for (int i = 0; i < count; i++) { _indices[i] = start + i; }
        }

        #endregion

        #region Public Methods

        //     -----     Implement IVariable     -----     //

        /// <inheritdoc/>
        public int ReferenceIndex(int index) => ReferenceVariable.ReferenceIndex(_indices[index]);

        /// <summary>
        /// Provides with the translation of the component's indices into the component's indices of the <see cref="ReferenceVariable"/>.
        /// </summary>
        /// <returns> The list of indices. </returns>
        public IReadOnlyList<int> ReferenceIndices() => _indices;


        /// <inheritdoc/>
        public double[] ToArray()
        {
            double[] components = new double[Size];
            for (int i = 0; i < Size; i++)
            {
                components[i] = ReferenceVariable[_indices[i]];
            }

            return components;
        }


        //     -----     Implement IEquatable<.>     -----     //

        /// <summary> Indicates whether this <see cref="Variable"/> is equal to another <see cref="Variable"/>. </summary>
        /// <param name="other"> A <see cref="Variable"/> to compare with this one. </param>
        /// <returns> <see langword="true"/> if their <see cref="ArraySegment{T}"/> field are equal, <see langword="false"/> otherwise. </returns>
        public bool Equals(SubVariable? other) => ReferenceVariable.Equals(other!.ReferenceVariable) & _indices.Equals(other._indices);

        #endregion


        #region Override : Object

        /// <inheritdoc cref="Object.Equals(object)"/>
        public override bool Equals(object? obj)
        {
            return obj is SubVariable subVariable && Equals(subVariable);
        }

        /// <inheritdoc cref="Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            int hashCode = -924498043;
            hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(_indices);
            hashCode = hashCode * -1521134295 + EqualityComparer<Variable>.Default.GetHashCode(ReferenceVariable);
            return hashCode;
        }

        #endregion
    }
}
