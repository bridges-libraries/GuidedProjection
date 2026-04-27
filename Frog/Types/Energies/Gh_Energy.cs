using System;
using System.Collections.Generic;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_Var = Frog.Types.Variables;


namespace Frog.Types.Energies
{
    /// <summary>
    /// Class defining a grasshopper type, representing a <see cref="GP.Energy"/>, enhanced with additional information.
    /// </summary>
    public class Gh_Energy : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Grasshopper variables composing the local vector localX on which the <see cref="EnergyType"/> is defined.
        /// </summary>
        private readonly Types_Var.IGH_GooVariable[] _variables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of this grasshopper energy.
        /// </summary>
        public int Size
        {
            get
            {
                return EnergyType.Value.LocalKi.Size;
            }
        }


        /// <summary>
        /// Gets the name of the set to which this grasshopper energy belongs.
        /// </summary>
        public string SetName { get; init; }


        /// <summary>
        /// Gets the grasshopper variables composing the local vector localX.
        /// </summary>
        public IReadOnlyList<Types_Var.IGH_GooVariable> Variables => _variables;

        /// <summary>
        /// Gets the grasshopper energy type which defines the local constraint members.
        /// </summary>
        public Gh_IEnergyType EnergyType { get; private set; }

        /// <summary>
        /// Gets or sets the weight of this grasshopper energy.
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
            _variables = new Types_Var.IGH_GooVariable[0];

            // Initialise Properties
            SetName = string.Empty;

            EnergyType = new Gh_IEnergyType();
            Weight = 0;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from its members.
        /// </summary>
        /// <param name="gh_Variables"> Grasshopper variables composing the local vector localX. </param>
        /// <param name="gh_EnergyType"> Energy type defining the local quantities of this energy. </param>
        /// <param name="weight"> Weight of this energy. </param>
        /// <param name="name"> Name of the set to which the grasshopper constraint belongs. </param>
        public Gh_Energy(IList<Types_Var.IGH_GooVariable> gh_Variables, Gh_IEnergyType gh_EnergyType, double weight, string name)
        {
            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Variables, nameof(gh_Variables));
            if (gh_Variables.Count == 0) { throw new ArgumentException("The list of variables is empty."); }

            if (gh_EnergyType.Value.LocalKi is null)
            {
                throw new ArgumentException($"The column vector {nameof(EnergyType.Value.LocalKi)} is null.");
            }

            int size_Variables = 0;
            for (int i = 0; i < gh_Variables.Count; i++) { size_Variables += gh_Variables[i].Size; }

            if (gh_EnergyType.Value.LocalKi is not null && gh_EnergyType.Value.LocalKi.Size != Size)
            {
                throw new ArgumentException($"The size of the energytype's column vector is not equal to the cumulated size of the {nameof(Variables)}");
            }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Types_Var.IGH_GooVariable[gh_Variables.Count];
            for (int i = 0; i < _variables.Length; i++) { _variables[i] = gh_Variables[i]; }

            // Initialise Properties
            SetName = name;

            EnergyType = gh_EnergyType;
            Weight = weight;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from another <see cref="Gh_Energy"/>.
        /// </summary>
        /// <param name="gh_Energy"> <see cref="Gh_Energy"/> to shallow copy. </param>
        public Gh_Energy(Gh_Energy gh_Energy)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _variables = new Types_Var.IGH_GooVariable[gh_Energy._variables.Length];
            Array.Copy(gh_Energy._variables, _variables, gh_Energy._variables.Length);

            // Initialise Properties
            SetName = gh_Energy.SetName;

            EnergyType = gh_Energy.EnergyType;
            Weight = gh_Energy.Weight;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString()
        {
            if (IsValid)
            {
                return $"Energy (Size: {Size})";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion


        #region Implement : GH_Types.IGH_Goo

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid
        {
            get
            {
                if (_variables.Length == 0) { return false; }
                else if (!EnergyType.IsValid) {return false; }
                else
                {
                    int size_Variables = 0;
                    for (int i = 0; i < _variables.Length; i++) { size_Variables += _variables[i].Size; }

                    return EnergyType.Size == size_Variables;
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
                        return "The list of energy variables is empty.";
                    }
                    else if (!EnergyType.IsValid)
                    {
                        return "The energy type is not valid.";
                    }
                    else if (EnergyType.Size != size_Variables)
                    {
                        return "Then cumulated size of the variables does not match the size of the energy type.";
                    }
                    else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription { get { return string.Format($"Grasshopper type representing a {typeof(GP.Energy)}."); } }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName { get { return nameof(Gh_Energy); } }


        // ---------- Methods ---------- //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate"/>
        public GH_Types.IGH_Goo Duplicate()
        {
            return new Gh_Energy(this);
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();


            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{Q}(ref Q)"/>
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
