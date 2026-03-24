using BRIDGES.Solvers.GuidedProjection.Abstracts;
using Frog.Types.Constraints;
using Frog.Types.Variables;
using Grasshopper.GUI;
using System;
using System.Collections.Generic;
using GH_Types = Grasshopper.Kernel.Types;
using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Energies
{
    /// <summary>
    /// Class defining a grasshopper type, which represents an energy.
    /// </summary>
    public class Gh_Energy : GH_Types.GH_Goo<GP.Energy>
    {
        #region Fields

        /// <summary>
        /// Generic grasshopper variables composing the local vector localX on which the <see cref="this.EnergyType"/> is defined.
        /// </summary>
        private readonly Gh_IVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of this grasshopper constraint.
        /// </summary>
        /// <remarks>
        /// It corresponds to the size of the square matrix <see cref="EnergyType"/>, to the size of the column vector <see cref="EnergyType"/>,
        /// as well as the cumulated size of the <see cref="Variables"/>. 
        /// </remarks>
        public int Size
        {
            get
            {
                if (EnergyType is not null)
                {
                    if (EnergyType.Value.LocalKi is not null && EnergyType.Value.LocalKi.Size != 0)
                    {
                        return EnergyType.Value.LocalKi.Size;
                    }
                    else { throw new Exception("The energy type is not valid."); }
                }
                else if (_variables is not null && _variables.Length != 0)
                {
                    int size = 0;
                    for (int i = 0; i < _variables.Length; i++) { size += _variables[i].Size; }

                    return size;
                }
                else
                {
                    throw new Exception("The variables and the energy type are not initialised. The size is therefore not defined.");
                }
            }
        }


        /// <summary>
        /// Gets the grasshopper variables composing the local vector localX.
        /// </summary>
        public IReadOnlyList<Gh_IVariable> Variables => _variables;

        /// <summary>
        /// Gets the Gh_IEnergyType containing an energy type which defines the local energy members:
        /// <list type="table">
        ///     <item>
        ///         <term> <see cref="GP.Interfaces.IEnergyType.LocalBi"/> </term>
        ///         <description> Column vector whose size is defined by the cumulated size of the <see cref="this.Variables"/>. </description>
        ///     </item>
        ///     <item>
        ///         <term> <see cref="GP.Interfaces.IEnergyType.Si"/> </term>
        ///         <description> Scalar value. </description>
        ///     </item>
        /// </list>
        /// </summary>
        public Gh_IEnergyType EnergyType { get; private set; }

        /// <summary>
        /// Gets or sets the weight of this Gh_Constraint.
        /// </summary>
        public double Weight { get; internal set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class.
        /// </summary>
        public Gh_Energy()
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Gh_IVariable[0];

            // Initialise Properties
            EnergyType = new Gh_IEnergyType();
            Weight = 0;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from its members.
        /// </summary>
        /// <param name="gh_Variables"> Variables composing the local vector localX. </param>
        /// <param name="gh_IEnergyType"> Energy type defining the local quantities of this energy. </param>
        /// <param name="weight"> Weight of this energy. </param>
        public Gh_Energy(IList<Gh_IVariable> gh_Variables, Gh_IEnergyType gh_IEnergyType, double weight)
        {
            //     -----     Verifications

            if (gh_Variables is null) { throw new ArgumentNullException(nameof(gh_Variables)); }
            else if (gh_Variables.Count == 0) { throw new ArgumentException("The list of variables is empty."); }

            if (gh_IEnergyType.Value.LocalKi is null)
            {
                throw new ArgumentException($"The column vector {nameof(EnergyType.Value.LocalKi)} is null.");
            }

            int size_Variables = 0;
            for (int i = 0; i < gh_Variables.Count; i++) { size_Variables += gh_Variables[i].Size; }

            if (gh_IEnergyType.Value.LocalKi is not null && gh_IEnergyType.Value.LocalKi.Size != Size)
            {
                throw new ArgumentException($"The size of the energytype's column vector is not equal to the cumulated size of the {nameof(Variables)}");
            }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Gh_IVariable[gh_Variables.Count];
            for (int i = 0; i < _variables.Length; i++) { _variables[i] = gh_Variables[i]; }

            // Initialise Properties
            EnergyType = gh_IEnergyType;
            Weight = weight;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from another <see cref="Gh_Energy"/>.
        /// </summary>
        /// <param name="gh_Energy"> <see cref="Gh_Energy"/> to duplicate. </param>
        public Gh_Energy(Gh_Energy gh_Energy)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Gh_IVariable[gh_Energy._variables.Length];
            Array.Copy(gh_Energy._variables, _variables, gh_Energy._variables.Length);

            // Initialise Properties
            EnergyType = gh_Energy.EnergyType;
            Weight = gh_Energy.Weight;
        }

        #endregion


        #region Override : GH_Types.GH_Goo<>

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
