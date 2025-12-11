using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Frog.Types.Energies
{
    /// <summary>
    /// Class defining a grasshopper type, which represents an energy.
    /// </summary>
    public class Gh_Energy : GH_Types.GH_Goo<GP.Energy>
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class.
        /// </summary>
        public Gh_Energy() { /* Do Nothing */ }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from another <see cref="Gh_Energy"/>.
        /// </summary>
        /// <param name="gh_Energy"> <see cref="Gh_Energy"/> to duplicate. </param>
        public Gh_Energy(Gh_Energy gh_Energy)
        {
            Value = gh_Energy.Value;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from a <see cref="GP.Energy"/>.
        /// </summary>
        /// <param name="energy"> <see cref="GP.Energy"/> to store. </param>
        public Gh_Energy(GP.Energy energy)
        {
            Value = energy;
        }

        #endregion


        #region Override : GH_Goo<>

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid => true;

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeDescription"/>
        public override string TypeDescription { get { return string.Format($"Grasshopper type containing a {typeof(GP.Energy)}."); } }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeName"/>
        public override string TypeName { get { return nameof(Gh_Energy); } }


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.Duplicate"/>
        public override GH_Types.IGH_Goo Duplicate()
        {
            return new Gh_Energy(this);
        }


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastFrom(object)"/>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            // ----- BRIDGES Objects ----- //

            // Cast a GP.Energy to a Gh_Energy
            if (typeof(GP.Energy).IsAssignableFrom(type))
            {
                Value = (GP.Energy)source;

                return true;
            }

            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {
            // ----- BRIDGES Objects ----- //

            // Casts a Gh_Energy to a GP.Energy
            if (typeof(T).IsAssignableFrom(typeof(GP.Energy)))
            {
                object energy = Value;
                target = (T)energy;

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
                return $"Energy (T:{Value.EnergyType})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion
    }
}
