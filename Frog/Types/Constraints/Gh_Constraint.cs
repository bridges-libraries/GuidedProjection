using System;
using System.Collections.Generic;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_Var = Frog.Types.Variables;


namespace Frog.Types.Constraints
{
    /// <summary>
    /// Class defining a grasshopper type, representing a <see cref="GP.Constraint"/>, enhanced with additional information.
    /// </summary>
    public class Gh_Constraint : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Grasshopper variables composing the local vector localX on which the <see cref="ConstraintType"/> is defined.
        /// </summary>
        private readonly Types_Var.IGH_GooVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of this grasshopper constraint.
        /// </summary>
        public int Size
        {
            get
            {
                return ConstraintType.Value.LocalHi is null ? ConstraintType.Value.LocalBi!.Size : ConstraintType.Value.LocalHi.RowCount;
            }
        }


        /// <summary>
        /// Gets the grasshopper variables composing the local vector localX.
        /// </summary>
        public IReadOnlyList<Types_Var.IGH_GooVariable> Variables => _variables;

        /// <summary>
        /// Gets the grasshopper constraint type which defines the local constraint members.
        /// </summary>
        public Gh_IConstraintType ConstraintType { get; private set; }

        /// <summary>
        /// Gets or sets the weight of this grasshopper constraint.
        /// </summary>
        public double Weight { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class.
        /// </summary>
        public Gh_Constraint()
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Types_Var.IGH_GooVariable[0];

            // Initialise Properties
            ConstraintType = new Gh_IConstraintType();
            Weight = 0;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class from its members.
        /// </summary>
        /// <param name="gh_Variables"> Grasshopper variables composing the local vector localX. </param>
        /// <param name="gh_ConstraintType"> Constraint type defining the local quantities of this constraint. </param>
        /// <param name="weight"> Weight of this constraint. </param>
        public Gh_Constraint(IList<Types_Var.IGH_GooVariable> gh_Variables, Gh_IConstraintType gh_ConstraintType, double weight)
        {
            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Variables, nameof(gh_Variables));
            if (gh_Variables.Count == 0) { throw new ArgumentException("The list of variables is empty."); }

            if (gh_ConstraintType.Value.LocalHi is null & gh_ConstraintType.Value.LocalBi is null)
            {
                throw new ArgumentException($"The square matrix {nameof(ConstraintType.Value.LocalHi)} and the column vector {nameof(ConstraintType.Value.LocalBi)} are both null.");
            }

            int size_Variables = 0;
            for (int i = 0; i < gh_Variables.Count; i++) { size_Variables += gh_Variables[i].Size; }


            if (gh_ConstraintType.Value.LocalHi is not null && (gh_ConstraintType.Value.LocalHi.RowCount != size_Variables | gh_ConstraintType.Value.LocalHi.ColumnCount != size_Variables))
            {
                throw new ArgumentException($"The size of the constraint type's square matrix is not equal to the cumulated size of the {nameof(Variables)}");
            }
            if (gh_ConstraintType.Value.LocalBi is not null && gh_ConstraintType.Value.LocalBi.Size != size_Variables)
            {
                throw new ArgumentException($"The size of the constraint type's column vector  is not equal to the cumulated size of the {nameof(Variables)}");
            }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Types_Var.IGH_GooVariable[gh_Variables.Count];
            for (int i = 0; i < _variables.Length; i++) { _variables[i] = gh_Variables[i]; }

            // Initialise Properties
            ConstraintType = gh_ConstraintType;
            Weight = weight;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class from another <see cref="Gh_Constraint"/>.
        /// </summary>
        /// <param name="gh_Constraint"> <see cref="Gh_Constraint"/> to shallow copy. </param>
        public Gh_Constraint(Gh_Constraint gh_Constraint)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Types_Var.IGH_GooVariable[gh_Constraint._variables.Length]; 
            Array.Copy(gh_Constraint._variables, _variables, gh_Constraint._variables.Length);

            // Initialise Properties
            ConstraintType = gh_Constraint.ConstraintType;
            Weight = gh_Constraint.Weight;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString()
        {
            if (IsValid)
            {
                return $"Constraint (Size: {Size})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion


        #region Implement : GH_Types.IGH_Goo

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.IGH_Goo{T}.IsValid"/>
        public bool IsValid
        {
            get
            {
                if (_variables.Length == 0) { return false; }
                else if (!ConstraintType.IsValid) { return false; }
                else
                {
                    int size_Variables = 0;
                    for (int i = 0; i < _variables.Length; i++) { size_Variables += _variables[i].Size; }

                    return ConstraintType.Size == size_Variables;
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
                    int size_Variables = 0;
                    for (int i = 0; i < _variables.Length; i++) { size_Variables += _variables[i].Size; }

                    if (_variables.Length == 0) 
                    { 
                        return "The list of constraint variables is empty."; 
                    }
                    else if (!ConstraintType.IsValid)
                    {
                        return "The constraint type is not valid.";
                    }
                    else if (ConstraintType.Size != size_Variables)
                    {
                        return "Then cumulated size of the variables does not match the size of the constraint type.";
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
        public bool CastTo<T>(out T? target)
        {
            target = default;

            // ----- Otherwise ----- //

            return false;
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
