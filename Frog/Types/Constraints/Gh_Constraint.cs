using System;
using System.Collections.Generic;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;

using Frog.Types.Variables;


namespace Frog.Types.Constraints
{
    /// <summary>
    /// Class defining a grasshopper type, representing a <see cref="GP.Constraint"/>, enhanced with additional information.
    /// </summary>
    public class Gh_Constraint : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Gh_IVariables composing the local vector localX on which the <see cref="this.ConstraintType"/> is defined.
        /// </summary>
        private readonly Gh_IVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Size of the Gh_Constraint.
        /// </summary>
        /// <remarks> It corresponds to the size of the square matrix <see cref="IConstraintType.LocalHi"/>, to the size of the column vector <see cref="IConstraintType.LocalBi"/>,
        /// as well as the cumulated size of the <see cref="Variables"/>. </remarks>
        public int Size
        {
            get
            {
                if (ConstraintType is not null)
                {
                    if (ConstraintType.Value.LocalHi is not null && ConstraintType.Value.LocalHi.RowCount != 0) 
                    {
                        return ConstraintType.Value.LocalHi.RowCount;
                    }
                    else if (ConstraintType.Value.LocalBi is not null && ConstraintType.Value.LocalBi.Size != 0)
                    {
                        return ConstraintType.Value.LocalBi.Size;
                    }
                    else { throw new Exception("The constraint type is not valid."); }
                }
                else if (_variables is not null && _variables.Length != 0)
                {
                    int size = 0;
                    for (int i = 0; i < _variables.Length; i++) { size += _variables[i].Value.Size; }

                    return size;
                }
                else
                {
                    throw new Exception("The variables and the constraint type are not initialised. The size is therefore not defined.");
                }
            }
        }


        /// <summary>
        /// Gets the Gh_IVariables composing the local vector localX.
        /// </summary>
        public IReadOnlyList<Gh_IVariable> Variables => _variables;


        /// /// <summary>
        /// Gets the Gh_IConstraintType containing a constraint type which defines the local constraint members:
        /// <list type="table">
        ///     <item>
        ///         <term> <see cref="IConstraintType.LocalHi"/> </term>
        ///         <description> Square matrix whose size is defined by the cumulated size of the <see cref="Variables"/>. </description>
        ///     </item>
        ///     <item>
        ///         <term> <see cref="IConstraintType.LocalBi"/> </term>
        ///         <description> Column vector whose size is defined by the cumulated size of the <see cref="Variables"/>. </description>
        ///     </item>
        ///     <item>
        ///         <term> <see cref="IConstraintType.Ci"/> </term>
        ///         <description> Scalar value. </description>
        ///     </item>
        /// </list>
        /// </summary>
        public Gh_IConstraintType ConstraintType { get; private set; }

        /// <summary>
        /// Gets or sets the weight of this Gh_Constraint.
        /// </summary>
        public double Weight { get; internal set; }



        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class.
        /// </summary>
        public Gh_Constraint() 
        {
            // Initialise Fields
            _variables = new Gh_IVariable[0];

            // Initialise Properties
            ConstraintType = new Gh_IConstraintType();
            Weight = 0;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class from another <see cref="Gh_Constraint"/>.
        /// </summary>
        /// <param name="gh_Constraint"> <see cref="Gh_Constraint"/> to shallow copy. </param>
        public Gh_Constraint(Gh_Constraint gh_Constraint)
        {
            // Initialise Fields
            _variables = new Gh_IVariable[gh_Constraint._variables.Length]; 
            Array.Copy(gh_Constraint._variables, _variables, gh_Constraint._variables.Length);

            // Initialise Properties
            ConstraintType = gh_Constraint.ConstraintType;
            Weight = gh_Constraint.Weight;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class from another <see cref="Gh_Constraint"/>.
        /// </summary>
        /// <param name="gh_Variables"> Variables composing the local vector localX. </param>
        /// <param name="gh_IConstraintType"> Constraint type defining the local quantities of this constraint. </param>
        /// <param name="weight"> Weight of this constraint. </param>
        public Gh_Constraint(IList<Gh_IVariable> gh_Variables, Gh_IConstraintType gh_IConstraintType, double weight)
        {
            //     -----     Verifications

            if (gh_Variables is null) { throw new ArgumentNullException(nameof(gh_Variables)); }
            else if (gh_Variables.Count == 0) { throw new ArgumentException("The list of variables is empty."); }

            if (gh_IConstraintType.Value.LocalHi is null & gh_IConstraintType.Value.LocalBi is null)
            {
                throw new ArgumentException($"The square matrix {nameof(ConstraintType.Value.LocalHi)} and the column vector {nameof(ConstraintType.Value.LocalBi)} cannot be both null.");
            }

            int size_Variables = 0;
            for (int i = 0; i < gh_Variables.Count; i++) { size_Variables += gh_Variables[i].Value.Size; }


            if (gh_IConstraintType.Value.LocalHi is not null && (gh_IConstraintType.Value.LocalHi.RowCount != Size | gh_IConstraintType.Value.LocalHi.ColumnCount != Size))
            {
                throw new ArgumentException($"The size of the constraint type's square matrix is not equal to the cumulated size of the {nameof(Variables)}");
            }
            if (gh_IConstraintType.Value.LocalBi is not null && gh_IConstraintType.Value.LocalBi.Size != Size)
            {
                throw new ArgumentException($"The size of the constraint type's column vector  is not equal to the cumulated size of the {nameof(Variables)}");
            }


            // Initialise Fields
            _variables = new Gh_IVariable[gh_Variables.Count];
            for (int i = 0; i < _variables.Length; i++) {  _variables[i] = gh_Variables[i]; }

            // Initialise Properties
            ConstraintType = gh_IConstraintType;
            Weight = weight;
        }

        #endregion


        #region Override : IGH_Goo<>

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.IsValid"/>
        public bool IsValid
        {
            get
            {
                if (_variables.Length == 0) { return false; }
                else
                {
                    int size_ConstraintType = 0, size_Variables = 0;

                    if (ConstraintType.Value.LocalHi is not null && ConstraintType.Value.LocalHi.RowCount != 0)
                    {
                        size_ConstraintType = ConstraintType.Value.LocalHi.RowCount;
                    }
                    else if (ConstraintType.Value.LocalBi is not null && ConstraintType.Value.LocalBi.Size != 0)
                    {
                        size_ConstraintType = ConstraintType.Value.LocalBi.Size;
                    }
                    else { throw new Exception("The constraint type is not valid."); }

                    for (int i = 0; i < _variables.Length; i++) { size_Variables += _variables[i].Value.Size; }

                    return size_ConstraintType == size_Variables;
                }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if ( (ConstraintType.Value.LocalHi is null || ConstraintType.Value.LocalHi.RowCount == 0)
                        && (ConstraintType.Value.LocalBi is null || ConstraintType.Value.LocalBi.Size == 0))
                    {
                        return "The constraint type is not valid.";
                    }
                    else if (_variables.Length == 0) 
                    { 
                        return "The set of constraint variables is empty."; 
                    }
                    else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                }
            }
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.TypeDescription"/>
        public string TypeDescription { get { return string.Format($"Grasshopper type representing a {typeof(GP.Constraint)}."); } }

        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.TypeName"/>
        public string TypeName { get { return nameof(Gh_Constraint); } }


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.Duplicate"/>
        public GH_Types.IGH_Goo Duplicate()
        {
            return new Gh_Constraint(this);
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.CastTo{Q}(ref Q)"/>
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

        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;

        #endregion

        #region Override : Object

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                 return $"Constraint (Size: {Size})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion
    }
}
