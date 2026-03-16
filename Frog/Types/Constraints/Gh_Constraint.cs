using BRIDGES.Solvers.GuidedProjection.Interfaces;
using Frog.Types.Variables;
using System;
using System.Collections.Generic;
using GH_Types = Grasshopper.Kernel.Types;
using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Constraints
{
    /// <summary>
    /// Class defining a grasshopper type, representing a <see cref="GP.Constraint"/>.
    /// </summary>
    public class Gh_Constraint : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Gh_IVariables composing the local vector localX on which the <see cref="Gh_IConstraintType"/> is defined.
        /// </summary>
        private readonly Gh_IVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the cumulated size of the <see cref="Variables"/>
        /// </summary>
        /// <remarks> It corresponds to the size of the square matrix <see cref="IConstraintType.LocalHi"/> and of the column vector <see cref="IConstraintType.LocalBi"/>. </remarks>
        public int Size { get; init; }


        /// /// <summary>
        /// Gets the constraint type defining the local constraint members
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
        public Gh_IConstraintType? ConstraintType { get; private set; }

        /// <summary>
        /// Gets the variables composing the local vector localX.
        /// </summary>
        public IReadOnlyList<Gh_IVariable> Variables => _variables;


        /// <summary>
        /// Gets or sets the weight of this constraint.
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
            ConstraintType = null;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class from another <see cref="Gh_Constraint"/>.
        /// </summary>
        /// <param name="gh_Constraint"> <see cref="Gh_Constraint"/> to duplicate. </param>
        public Gh_Constraint(Gh_Constraint gh_Constraint)
        {
            Value = gh_Constraint.Value;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class from another <see cref="Gh_Constraint"/>.
        /// </summary>
        /// <param name="constraint"> <see cref="GP.Constraint"/> to store. </param>
        public Gh_Constraint(GP.Constraint constraint)
        {
            Value = constraint;
        }

        #endregion


        #region Override : GH_Goo<>

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid => true;

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeDescription"/>
        public override string TypeDescription { get { return string.Format($"Grasshopper type containing a {typeof(GP.Constraint)}."); } }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeName"/>
        public override string TypeName { get { return nameof(Gh_Constraint); } }


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.Duplicate"/>
        public override GH_Types.IGH_Goo Duplicate()
        {
            return new Gh_Constraint(this);
        }


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastFrom(object)"/>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            // ----- BRIDGES Objects ----- //

            // Cast a GP.Constraint to a Gh_Constraint
            if (typeof(GP.Constraint).IsAssignableFrom(type))
            {
                Value = (GP.Constraint)source;

                return true;
            }


            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {
            // ----- BRIDGES Objects ----- //

            // Casts a Gh_Constraint to a GP.Constraint
            if (typeof(T).IsAssignableFrom(typeof(GP.Constraint)))
            {
                object constraint = Value;
                target = (T)constraint;

                return true;
            }

            // ----- Otherwise ----- //

            return false;
        }

        #endregion

        #region Override : Object

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                 return $"Constraint (T:{Value.ConstraintType}; S:{Value.Size})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion
    }
}
