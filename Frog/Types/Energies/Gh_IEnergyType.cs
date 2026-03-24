using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Frog.Types.Energies
{
    /// <summary>
    /// Class defining a grasshopper type, containing an <see cref="GP.Interfaces.IEnergyType"/>.
    /// </summary>
    public class Gh_IEnergyType : GH_Types.GH_Goo<GP.Interfaces.IEnergyType>
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IEnergyType" /> class.
        /// </summary>
        public Gh_IEnergyType() { /* Do Nothing */ }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IEnergyType" /> class from a <see cref="GP.Interfaces.IEnergyType"/>.
        /// </summary>
        /// <param name="iEnergyType"> <see cref="GP.Interfaces.IEnergyType"/> to store. </param>
        public Gh_IEnergyType(GP.Interfaces.IEnergyType iEnergyType)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            Value = iEnergyType;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IEnergyType" /> class from another <see cref="Gh_IEnergyType"/>.
        /// </summary>
        /// <param name="gh_IEnergyType"> <see cref="Gh_IEnergyType"/> to shallow copy. </param>
        public Gh_IEnergyType(Gh_IEnergyType gh_IEnergyType)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            Value = gh_IEnergyType.Value;
        }

        #endregion


        #region Override : GH_Types.GH_Goo<>

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid => m_value is not null && m_value.LocalKi?.Size != 0;

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValidWhyNot"/>
        public override string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else if (m_value is null)
                {
                    return "The contained IEnergyType is null";
                }
                else if (m_value.LocalKi?.Size == 0)
                {
                    return "The local vector LocalBi of the contained IEnergyType is null or empty.";
                }
                else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
            }
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeDescription"/>
        public override string TypeDescription { get { return string.Format($"Grasshopper type containing a {typeof(GP.Interfaces.IEnergyType)}."); } }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeName"/>
        public override string TypeName { get { return nameof(Gh_IEnergyType); } }


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.Duplicate"/>
        public override GH_Types.IGH_Goo Duplicate()
        {
            return new Gh_IEnergyType(this);
        }


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastFrom(object)"/>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            // ----- BRIDGES Objects ----- //

            // Cast a GP.Interfaces.IEnergyType to a Gh_IEnergyType
            if (typeof(GP.Interfaces.IEnergyType).IsAssignableFrom(type))
            {
                Value = (GP.Interfaces.IEnergyType)source;

                return true;
            }


            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {
            // ----- BRIDGES Objects ----- //

            // Casts a Gh_IEnergyType to a GP.Interfaces.IEnergyType
            if (typeof(T).IsAssignableFrom(typeof(GP.Interfaces.IEnergyType)))
            {
                object energyType = Value;
                target = (T)energyType;

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
                int size = m_value.LocalKi!.Size ;

                 return $"IEnergyType (Size: {size})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion
    }
}
