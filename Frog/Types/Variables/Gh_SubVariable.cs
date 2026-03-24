using System;
using System.Collections.Generic;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Class defining a grasshopper type which contains a <see cref="GP.SubVariable"/>, enhanced with additional information.
    /// </summary>
    public class Gh_SubVariable : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Parent grasshopper variable of this grasshopper subvariable.
        /// </summary>
        private readonly Gh_Variable _gh_variable;

        /// <summary>
        /// Indices of the <see cref="_gh_variable"/>, retained for this grasshopper subvariable.
        /// </summary>
        private readonly int[] _indices;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent grasshopper variable of this grasshopper subvariable.
        /// </summary>
        public Gh_Variable ParentVariable => _gh_variable;

        /// <summary>
        /// Gets the indices of the variable's coordinates retained for the subvariable.
        /// </summary>
        public IReadOnlyList<int> RetaindeIndices => _indices;


        /// <summary>
        /// Gets the size of this grasshopper subvariable.
        /// </summary>
        public int Size => _indices.Length;

        /// <summary>
        /// Gets or set the coordinate value at the given index.
        /// </summary>
        /// <param name="index"> Zero-based index of the coordinate. </param>
        /// <returns> The <see langword="double"/>-precision value of the coordinate. </returns>
        public double this[int index] => _gh_variable[_indices[index]];

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_SubVariable" /> class.
        /// </summary>
        public Gh_SubVariable()
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _gh_variable = new Gh_Variable();
            _indices = new int[0];
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_SubVariable" /> class from a parent <see cref="Gh_Variable"/>.
        /// </summary>
        /// <param name="gh_Variable"> Parent variable containing the reference coordinates of the subvariable. </param>
        /// <param name="indices"> Indices of the variable's coordinates retained for the subvariable. </param>
        public Gh_SubVariable(Gh_Variable gh_Variable, IList<int> indices)
        {
            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Variable, nameof(gh_Variable));
            ArgumentNullException.ThrowIfNull(indices, nameof(indices));

            if (!gh_Variable.IsValid) { throw new ArgumentException("The reference variable is not valid."); }
            if (indices.Count == 0) { throw new ArgumentException("The list of retained indicesis empty."); }
            if (!(indices.Count < gh_Variable.Size)) 
            { 
                throw new ArgumentException("The list of retained indices is equal or larger than the size of the reference variable.");
            }


            bool[] isUsed = new bool[gh_Variable.Size];
            List<int> sortedIndices = new List<int>(indices);
            for (int i = 0; i < sortedIndices.Count; i++)
            {
                if (!isUsed[sortedIndices[i]]) { isUsed[sortedIndices[i]] = true; }
                else { throw new ArgumentException("The list of retained indices has duplicate number"); }
            }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _gh_variable = gh_Variable;

            _indices = new int[sortedIndices.Count];
            for (int i = 0; i < _indices.Length; i++) { _indices[i] = indices[i]; }
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_SubVariable" /> class from another <see cref="Gh_SubVariable"/>.
        /// </summary>
        /// <param name="gh_Variable"> <see cref="Gh_SubVariable"/> to shallow copy. </param>
        public Gh_SubVariable(Gh_SubVariable gh_SubVariable)
        {
            //     -----     Verifications     -----     //

            if (!gh_SubVariable.IsValid) { throw new ArgumentException($"The {typeof(Gh_SubVariable)} to duplicate is not valid."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _gh_variable = gh_SubVariable._gh_variable;

            _indices = new int[gh_SubVariable._indices.Length];
            for (int i = 0; i < _indices.Length; i++) { _indices[i] = gh_SubVariable._indices[i]; }
        }

        #endregion


        #region Implement : GH_Types.IGH_Goo

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => _gh_variable.IsValid & _indices.Length != 0;

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if (!_gh_variable.IsValid) { return "The reference variable is not valid."; }
                    else if (_indices.Length == 0) { return "The list of retained coordinates is empty."; }
                    else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => $"Grasshopper type representing a {typeof(GP.SubVariable)}.";

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_SubVariable);


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_SubVariable(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{Q}(ref Q)"/>
        public bool CastTo<T>(out T? target)
        {
            target = default;

            //     -----     Otherwise     -----     //

            return false;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                string text = "SubVariable";

                text += ": [" + this[0];
                for (int i = 1; i < this.Size; i++)
                {
                    text += ", " + this[i];
                }
                text += "]";

                return text;
            }
            else { return $"Invalid {TypeName}"; }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;


        /// <inheritdoc cref="GH_Types.IGH_Goo.EmitProxy()"/>
        public GH_Types.IGH_GooProxy? EmitProxy() => null;

        #endregion
    }
}
