using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Frog.Types.Constraints
{
    /// <summary>
    /// Class defining a grasshopper type, which represents a constraint.
    /// </summary>
    public class Gh_Constraint : GH_Types.GH_Goo<GP.Constraint>
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Constraint" /> class.
        /// </summary>
        public Gh_Constraint() { /* Do Nothing */ }

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
