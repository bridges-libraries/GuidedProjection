using System;
using System.Collections.Generic;
using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Models
{
    /// <summary>
    /// Class defining a grasshopper type, which represents a model.
    /// </summary>
    public class Gh_Model : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Sets of variables of the model.
        /// </summary>
        private Dictionary<string, List<GP.Variable>> _setsOfVariables = new Dictionary<string, List<GP.Variable>>();

        /// <summary>
        /// Constraints of the model.
        /// </summary>
        private Dictionary<string, List<GP.Constraint>> _setsOfConstraints = new Dictionary<string, List<GP.Constraint>>();

        /// <summary>
        /// Energies of the model.
        /// </summary>
        private Dictionary<string, List<GP.Energy>> _setsOfEnergies = new Dictionary<string, List<GP.Energy>>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of variables of the model.
        /// </summary>
        public int VariableCount { get; init; }

        /// <summary>
        /// Gets the number of constraints of the model.
        /// </summary>
        public int ConstraintCount { get; init; }

        /// <summary>
        /// Gets the number of energies of the model.
        /// </summary>
        public int EnergyCount { get; init; }


        /// <summary>
        /// Gets the sets of variables of the model.
        /// </summary>
        public IReadOnlyDictionary<string, List<GP.Variable>> SetsOfVariables => _setsOfVariables;

        /// <summary>
        /// Gets the sets of variables of the model.
        /// </summary>
        public IReadOnlyDictionary<string, List<GP.Constraint>> SetsOfConstraints => _setsOfConstraints;

        /// <summary>
        /// Gets the sets of variables of the model.
        /// </summary>
        public IReadOnlyDictionary<string, List<GP.Energy>> SetsOfEnergies => _setsOfEnergies;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class.
        /// </summary>
        public Gh_Model() { /* Do Nothing */}

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class from another <see cref="Gh_Model"/>.
        /// </summary>
        /// <param name="gh_Model"> <see cref="Gh_Model"/> to duplicate. </param>
        public Gh_Model(Gh_Model gh_Model)
        {
            //     -----     Initialise Fields

            _setsOfVariables = new Dictionary<string, List<GP.Variable>>(gh_Model.SetsOfVariables);
            _setsOfConstraints = new Dictionary<string, List<GP.Constraint>>(gh_Model.SetsOfConstraints);
            _setsOfEnergies = new Dictionary<string, List<GP.Energy>>(gh_Model.SetsOfEnergies);


            //     -----     Initialise Properties

            VariableCount = 0;
            foreach (List<GP.Variable> set in _setsOfVariables.Values) { VariableCount += set.Count; }

            ConstraintCount = 0;
            foreach (List<GP.Constraint> set in _setsOfConstraints.Values) { ConstraintCount += set.Count; }

            EnergyCount = 0;
            foreach (List<GP.Energy> set in _setsOfEnergies.Values) { EnergyCount += set.Count; }
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class from another <see cref="Gh_Model"/>.
        /// </summary>
        /// <param name="variableCount"> Number of variables of the model. </param>
        /// <param name="constraintCount"> Number of constriaints of the model. </param>
        /// <param name="energyCount"> Number of energies of the model. </param>
        /// <param name="sets"> Sets of variables of the model. </param>
        public Gh_Model(ref Dictionary<string, List<GP.Variable>> variableSets, ref Dictionary<string, List<GP.Constraint>> constraintsSets, ref Dictionary<string, List<GP.Energy>> energySets)
        {
            //     -----     Initialise Fields

            _setsOfVariables = variableSets;
            _setsOfConstraints = constraintsSets;
            _setsOfEnergies = energySets;


            //     -----     Initialise Properties

            VariableCount = 0;
            foreach (List<GP.Variable> set in _setsOfVariables.Values) { VariableCount += set.Count; }

            ConstraintCount = 0;
            foreach (List<GP.Constraint> set in _setsOfConstraints.Values) { ConstraintCount += set.Count; }

            EnergyCount = 0;
            foreach (List<GP.Energy> set in _setsOfEnergies.Values) { EnergyCount += set.Count; }
        }

        #endregion


        #region Implement : IGH_Goo<.>

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid
        {
            get 
            { 
                return _setsOfVariables?.Count != 0 & _setsOfConstraints?.Count != 0 & _setsOfEnergies?.Count != 0; 
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) 
                {
                    return string.Empty; 
                }
                else if (_setsOfVariables?.Count == 0)
                {
                    return $"The model does not contain any variables.";
                }
                else if (_setsOfConstraints?.Count == 0)
                {
                    return $"The model does not contain any constraint.";
                }
                else if (_setsOfEnergies?.Count == 0)
                {
                    return $"The model does not contain any energies.";
                }
                else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                    
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_Model);

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => string.Format($"Grasshopper type representing a computed model using the Guided Projection Algorithm.");

        
        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate()"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_Model(this);

        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable()
        {
            return new object[3] { _setsOfVariables, _setsOfConstraints, _setsOfEnergies };
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            // ----- Otherwise ----- //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{T}(out T)"/>
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


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        #endregion


        #region Override : Object

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                return $"GPA Model (V:{VariableCount}; C:{ConstraintCount}; E:{EnergyCount})";
            }
            else { return "Empty Gh_Model"; }
        }

        #endregion
    }
}
