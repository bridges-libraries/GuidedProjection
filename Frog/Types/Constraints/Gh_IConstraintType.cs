using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Frog.Types.Constraints
{
    /// <summary>
    /// Class defining a grasshopper type, containing an <see cref="GP.Interfaces.IConstraintType"/>.
    /// </summary>
    public class Gh_IConstraintType : GH_Types.GH_Goo<GP.Interfaces.IConstraintType>
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_EnergyType" /> class.
        /// </summary>
        public Gh_IConstraintType() { /* Do Nothing */ }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_EnergyType" /> class from a <see cref="GP.Interfaces.IConstraintType"/>.
        /// </summary>
        /// <param name="iConstraintType"> <see cref="GP.Interfaces.IConstraintType"/> to store. </param>
        public Gh_IConstraintType(GP.Interfaces.IConstraintType iConstraintType)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            m_value = iConstraintType;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_EnergyType" /> class from another <see cref="Gh_EnergyType"/>.
        /// </summary>
        /// <param name="gh_IConstraintType"> <see cref="Gh_IConstraintType"/> to shallow copy. </param>
        public Gh_IConstraintType(Gh_IConstraintType gh_IConstraintType)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            m_value = gh_IConstraintType.Value;
        }

        #endregion


        #region Override : GH_Types.GH_Goo<>

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid => m_value is not null && (m_value.LocalHi?.RowCount != 0 | m_value.LocalBi?.Size != 0);

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValidWhyNot"/>
        public override string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else if (m_value is null)
                {
                    return "The contained IConstraintType is null";
                }
                else if (m_value.LocalHi?.RowCount ==0 & m_value.LocalBi?.Size == 0)
                {
                    return "The symmetric square matrix LocalHi and the vector LocalBi of the contained IConstraintType are simultaneously null or empty.";
                }
                else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
            }
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeDescription"/>
        public override string TypeDescription { get { return string.Format($"Grasshopper type containing a {typeof(GP.Interfaces.IConstraintType)}."); } }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeName"/>
        public override string TypeName { get { return nameof(Gh_IConstraintType); } }


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.Duplicate"/>
        public override GH_Types.IGH_Goo Duplicate()
        {
            return new Gh_IConstraintType(this);
        }


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastFrom(object)"/>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            // ----- BRIDGES Objects ----- //

            // Cast a GP.Interfaces.IConstraintType to a Gh_IConstraintType
            if (typeof(GP.Interfaces.IConstraintType).IsAssignableFrom(type))
            {
                Value = (GP.Interfaces.IConstraintType)source;

                return true;
            }


            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {
            // ----- BRIDGES Objects ----- //

            // Casts a Gh_IConstraintType to a GP.Interfaces.IConstraintType
            if (typeof(T).IsAssignableFrom(typeof(GP.Interfaces.IConstraintType)))
            {
                object constraintType = Value;
                target = (T)constraintType;

                return true;
            }

            // ----- Otherwise ----- //

            return false;
        }


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                int size = m_value.LocalHi?.RowCount != 0 ? m_value.LocalHi!.RowCount : m_value.LocalBi!.Size;

                return $"IConstraintType (Size: {size})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion
    }
}
