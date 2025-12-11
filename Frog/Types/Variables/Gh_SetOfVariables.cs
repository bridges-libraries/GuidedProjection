using System;
using System.Collections;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Class defining a grasshopper type containing an indexed collection of <see cref="GP.Variable"/>.
    /// </summary>
    public class Gh_SetOfVariables : GH_Types.IGH_Goo,
        IEnumerable<GP.Variable>
    {
        #region Fields

        /// <summary>
        /// Variables from the set.
        /// </summary>
        private List<GP.Variable> _variables = new List<GP.Variable>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the set of variables.
        /// </summary>
        public string? Name { get; private set; }


        /// <summary>
        /// Gets the number of variables in the set.
        /// </summary>
        public int Count => _variables.Count;

        /// <summary>
        /// Gets the variable at the given index in the set.
        /// </summary>
        /// <param name="index"> Zero-based Index of the set variable to get. </param>i
        /// <returns> The variable at the given index in the set. </returns>
        public GP.Variable this[int index] => _variables[index];


        // ---------- Implement IGH_Goo ---------- //

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_SetOfVariables" /> class.
        /// </summary>
        public Gh_SetOfVariables() { /* Do Nothing */ }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_SetOfVariables" /> class from another <see cref="Gh_SetOfVariables"/>.
        /// </summary>
        /// <param name="gh_SetOfVariables"> <see cref="Gh_SetOfVariables"/> to deep copy. </param>
        public Gh_SetOfVariables(Gh_SetOfVariables gh_SetOfVariables)
        {
            //     -----     Initialise fields

            _variables = new List<GP.Variable>(gh_SetOfVariables._variables);


            //     -----     Initialise properties

            Name = gh_SetOfVariables.Name;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_SetOfVariables" /> class from an indexed collection of <see cref="GP.Variable"/>.
        /// </summary>
        /// <param name="name"> Custom name of the set. </param>
        /// <param name="variables"> Variables of the set. </param>
        public Gh_SetOfVariables(string name, IReadOnlyList<GP.Variable> variables)
        {
            //     -----     Initialise fields

            _variables = new List<GP.Variable>(variables);

            //     -----     Initialise properties

            Name = name;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="GH_Types.IGH_Goo.ToString()"/>
        public override string ToString()
        {
            if (IsValid)
            {
                return $"{Name} (C:{Count})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion


        #region Implement : IGH_Goo<.>

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => _variables.Count != 0 & (Name is not null && Name != string.Empty);

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else if (_variables.Count == 0)
                {
                    return $"The set of variables is empty.";
                }
                else if (Name is not null && Name != string.Empty)
                {
                    return $"The set name is null or empty.";
                }
                else { return $"This {TypeName} is not valid, but I don't know why"; }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_SetOfVariables);

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => string.Format($"Grasshopper type representing a set of indexed variables.");


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate()"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_SetOfVariables(this);

        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable()
        {
            return _variables;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{T}(out T)"/>
        public bool CastTo<T>(out T target)
        {
            target = default;


            // ----- Otherwise ----- //

            return false;
        }


#pragma warning disable CS8603 // Possible null reference return.

        /// <inheritdoc cref="GH_Types.IGH_Goo.EmitProxy()"/>
        public GH_Types.IGH_GooProxy EmitProxy() => null;

#pragma warning restore CS8603 // Possible null reference return.


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        #endregion

        #region Implement IEnumerable<.>

        //     -----     Methods     -----     //

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<GP.Variable> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _variables[i];
            }
        }

        #endregion


        #region Explicit Implementation : IEnumerable

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
