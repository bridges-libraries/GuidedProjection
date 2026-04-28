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
        private Dictionary<string, GP.Variable[]> _setsOfVariables = new Dictionary<string, GP.Variable[]>();

        /// <summary>
        /// Sets of constraints of the model.
        /// </summary>
        private Dictionary<string, List<GP.Constraint>> _setsOfConstraints = new Dictionary<string, List<GP.Constraint>>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of variables of the model.
        /// </summary>
        public int VariableCount 
        { 
            get
            {
                int count = 0;

                foreach (GP.Variable[] setVariables in _setsOfVariables.Values) { count += setVariables.Length; }

                return count;
            }

        }

        /// <summary>
        /// Gets the number of constraints of the model.
        /// </summary>
        public int ConstraintCount
        {
            get
            {
                int count = 0;

                foreach (List<GP.Constraint> setConstraints in _setsOfConstraints.Values) { count += setConstraints.Count; }

                return count;
            }

        }


        /// <summary>
        /// Gets the sets of variables of the model.
        /// </summary>
        public IReadOnlyDictionary<string, GP.Variable[]> SetsOfVariables => _setsOfVariables;

        /// <summary>
        /// Gets the sets of variables of the model.
        /// </summary>
        public IReadOnlyDictionary<string, List<GP.Constraint>> SetsOfConstraints => _setsOfConstraints;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class.
        /// </summary>
        public Gh_Model() { /* Do Nothing */ }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class from another <see cref="Gh_Model"/>.
        /// </summary>
        /// <param name="variableSets"> Sets of variables. </param>
        /// <param name="constraintSets"> Sets of constraints. </param>
        public Gh_Model(ref Dictionary<string, GP.Variable[]> variableSets, ref Dictionary<string, List<GP.Constraint>> constraintSets)
        {
            //     -----     Initialise Fields

            _setsOfVariables = variableSets;
            _setsOfConstraints = constraintSets;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class from another <see cref="Gh_Model"/>.
        /// </summary>
        /// <param name="gh_Model"> <see cref="Gh_Model"/> to duplicate. </param>
        public Gh_Model(Gh_Model gh_Model)
        {
            //     -----     Initialise Fields

            _setsOfVariables = new Dictionary<string, GP.Variable[]>(gh_Model.SetsOfVariables);
            _setsOfConstraints = new Dictionary<string, List<GP.Constraint>>(gh_Model.SetsOfConstraints);
        }

        #endregion


        #region Implement : IGH_Goo<.>

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid
        {
            get 
            { 
                return _setsOfVariables?.Count != 0 & _setsOfConstraints?.Count != 0; 
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
            return new object[2] { _setsOfVariables, _setsOfConstraints};
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
        public bool CastTo<T>(out T? target)
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
                return $"GPA Model (V:{VariableCount}; C:{ConstraintCount})";
            }
            else { return "Empty Gh_Model"; }
        }

        #endregion
    }
}
