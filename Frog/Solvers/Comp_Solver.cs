using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Models
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.Solver"/> model.
    /// </summary>
    public class Comp_Solver : GH_Kernel.GH_Component
    {
        #region Private Fields

        /// <summary>
        /// Sets of variables of the computed model.
        /// </summary>
        private Dictionary<string, List<GP.Variable>>? _setsOfVariables;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of variables of the computed model.  
        /// </summary>
        public int VariableCount { get; private set; }

        /// <summary>
        /// Gets the number of energies of the computed model.  
        /// </summary>
        public int EnergyCount { get; private set; }

        /// <summary>
        /// Gets the number of constraints of the computed model.  
        /// </summary>
        public int ConstraintCount { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Solver"/> class.
        /// </summary>
        public Comp_Solver()
          : base("GPA Solver", "GPA", "Creates and solves a model using the Guided Projection Algorithm (GPA).",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Solvers])
        {
            // Nothing To Do
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Contains the actual content for creating and solving the model with the Guided Projection Algorithm.
        /// </summary>
        /// <param name="DA"> Data Access object, used to retrieve the retrieve data from input parameter and assign data to output parameters. </param>
        /// <exception cref="ArgumentException"></exception>
        private void ActualSolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            List<Types.Variables.Gh_SetOfVariables> gh_SetsOfVariables = new List<Types.Variables.Gh_SetOfVariables>();
            List<Types.Constraints.Gh_Constraint> gh_Constraints = new List<Types.Constraints.Gh_Constraint>();
            List<Types.Energies.Gh_Energy> gh_Energies = new List<Types.Energies.Gh_Energy>();

            int iterationCount = 0;

            //     -----     Get Inputs     -----     //

            DA.GetDataList(0, gh_SetsOfVariables);
            if (!DA.GetDataList(1, gh_Constraints)) { return; }
            DA.GetDataList(2, gh_Energies);

            if (!DA.GetData(3, ref iterationCount)) { return; }

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Constraints);

            if (gh_Constraints.Count == 0) { throw new ArgumentException("The input C cannot be empty."); }


            //     -----     Core     -----     //

            double epsilon = 1e-5;

            GP.Solver gpa = CreateModel(epsilon, iterationCount, gh_SetsOfVariables, gh_Constraints, gh_Energies);

            gpa.InitialiseX();

            for (int i = 0; i < iterationCount; i++)
            {
                gpa.RunIteration();
            }
        }


        /// <summary>
        /// Creates a model for the Guided Projection Algorithm from the grasshopper inputs. 
        /// </summary>
        /// <param name="epsilon"></param>
        /// <param name="maxIteration"></param>
        /// <param name="gh_Variables"></param>
        /// <param name="gh_Constraints"></param>
        /// <param name="gh_Energies"></param>
        /// <returns></returns>
        private GP.Solver CreateModel(double epsilon, int maxIteration, List<Types.Variables.Gh_SetOfVariables> gh_SetsOfVariables, List<Types.Constraints.Gh_Constraint> gh_Constraints, List<Types.Energies.Gh_Energy> gh_Energies)
        {
            GP.Solver gpa = new GP.Solver(epsilon, maxIteration);

            AddVariablesToModel(gh_SetsOfVariables, ref gpa, out Dictionary<GP.Variable, GP.Variable> oldToNew);

            AddConstraintsToModel(gh_Constraints, ref gpa, ref oldToNew);
            AddEnergiesToModel(gh_Energies, ref gpa, ref oldToNew);


            VariableCount = gpa.VariableCount;
            ConstraintCount = gpa.ConstraintCount;
            EnergyCount = gpa.EnergyCount;

            return gpa;
        }


        /// <summary>
        /// Sorts and adds the input grasshopper variable to the model.
        /// </summary>
        /// <param name="gh_Variables"> The list of input grasshopper variables. </param>
        /// <param name="gpa"> The model to operate on. </param>
        /// <param name="oldToNew"> The dictionary translating the input variables into model variables. </param>
        /// <returns> A dictionary translating the input variables into model variables. </returns>
        private void AddVariablesToModel(List<Types.Variables.Gh_SetOfVariables> gh_SetsOfVariables, ref GP.Solver gpa, out Dictionary<GP.Variable, GP.Variable> oldToNew)
        {
            oldToNew = new Dictionary<GP.Variable, GP.Variable>();
            _setsOfVariables = new Dictionary<string, List<GP.Variable>>();

            for (int i_Set = 0; i_Set < gh_SetsOfVariables.Count; i_Set++)
            {
                Types.Variables.Gh_SetOfVariables gh_SetOfVariable = gh_SetsOfVariables[i_Set];

                //     -----     Verifications

                string? setName = gh_SetOfVariable.Name;
                if (setName is not null && setName != "")
                {
                    _setsOfVariables.Add(gh_SetOfVariable.Name!, new List<GP.Variable>(gh_SetOfVariable.Count));
                }
                else { throw new ArgumentException("A set of variables does not have a name."); }


                //     -----     Create the Model Variables

                for (int i_SetVariable = 0; i_SetVariable < gh_SetOfVariable.Count; i_SetVariable++)
                {
                    GP.Variable variable = gh_SetOfVariable[i_SetVariable];

                    if (oldToNew.ContainsKey(variable))
                    {
                        throw new ArgumentException("A variable is contained twice in the input S.");
                    }
                    else
                    {
                        GP.Variable modelVariable = gpa.AddVariable(variable.ToArray());

                        oldToNew.Add(variable, modelVariable);
                        _setsOfVariables[setName].Add(modelVariable);
                    }
                }
            }
        }

        /// <summary>
        /// Sorts and adds the input grasshopper constraints to the model.
        /// </summary>
        /// <param name="gh_Constraints"> The list of input grasshopper constraints. </param>
        /// <param name="gpa"> The model to operate on. </param>
        /// <param name="oldToNew"> The dictionary translating the input variables into model variables. </param>
        /// <exception cref="ArgumentException"> A constraint is contained twice in the input C. </exception>
        /// <exception cref="Exception"> A constraint's variable was not recognised as either a <see cref="GP.Variable"/> or a <see cref="GP.SubVariable"/>. </exception>
        private void AddConstraintsToModel(List<Types.Constraints.Gh_Constraint> gh_Constraints, ref GP.Solver gpa, ref Dictionary<GP.Variable, GP.Variable> oldToNew)
        {
            HashSet<GP.Constraint> constraints = new HashSet<GP.Constraint>();
            foreach (Types.Constraints.Gh_Constraint gh_Constraint in gh_Constraints)
            {
                GP.Constraint constraint = gh_Constraint.Value;

                if (constraints.Contains(constraint)) { throw new ArgumentException("A constraint is contained twice in the input C"); }

                //     -----     Manage constraint type

                GP.Interfaces.IConstraintType cloneConstraintType = (GP.Interfaces.IConstraintType)constraint.ConstraintType.Clone();


                //     -----     Manage constraint variables

                string constraintTypeName = constraint.ConstraintType.ToString() is not null ? constraint.ConstraintType.ToString()! : constraint.ConstraintType.GetType().ToString();

                List<GP.Interfaces.IVariable> modelIVariables = UpdateVariables(constraint.Variables, constraintTypeName, ref gpa, ref oldToNew);


                gpa.AddConstraint(cloneConstraintType, modelIVariables, constraint.Weight);
            }
        }

        /// <summary>
        /// Sorts and adds the input grasshopper energies to the model.
        /// </summary>
        /// <param name="gh_Energies"> The list of input grasshopper energies. </param>
        /// <param name="gpa"> The model to operate on. </param>
        /// <param name="oldToNew"> The dictionary translating the input variables into model variables. </param>
        /// <exception cref="ArgumentException"> An energy is contained twice in the input C. </exception>
        /// <exception cref="Exception"> An energy's variable was not recognised as either a <see cref="GP.Variable"/> or a <see cref="GP.SubVariable"/>. </exception>
        private void AddEnergiesToModel(List<Types.Energies.Gh_Energy> gh_Energies, ref GP.Solver gpa, ref Dictionary<GP.Variable, GP.Variable> oldToNew)
        {
            HashSet<GP.Energy> energies = new HashSet<GP.Energy>();
            foreach (Types.Energies.Gh_Energy gh_Energy in gh_Energies)
            {
                GP.Energy energy = gh_Energy.Value;

                if (energies.Contains(energy)) { throw new ArgumentException("An energy is contained twice in the input C"); }

                //     -----     Manage energy type

                GP.Interfaces.IEnergyType cloneEnergyType = (GP.Interfaces.IEnergyType)energy.EnergyType.Clone();

                //     -----     Manage energy variables

                string energyTypeName = energy.EnergyType.ToString() is not null ? energy.EnergyType.ToString()! : energy.EnergyType.GetType().ToString();

                List<GP.Interfaces.IVariable> modelIVariables = UpdateVariables(energy.Variables, energyTypeName, ref gpa, ref oldToNew);


                gpa.AddEnergy(cloneEnergyType, modelIVariables, energy.Weight);
            }
        }


        /// <summary>
        /// Creates a list of model variables from a list of input variables. 
        /// </summary>
        /// <remarks> Variable that are not contained in the model will be create and added. </remarks>
        /// <param name="variables"> Input variables to review.</param>
        /// <param name="setName"> Name of the set to use if a variable needs to be adde to the model </param>
        /// <param name="gpa"> The model to operate on. </param>
        /// <param name="oldToNew"> The dictionary translating the input variables into model variables. </param>
        /// <returns> The list of model variables, to replacethe input one. </returns>
        /// <exception cref="Exception"> A variable was not recognised as either a <see cref="GP.Variable"/> or a <see cref="GP.SubVariable"/>. </exception>
        private List<GP.Interfaces.IVariable> UpdateVariables(IReadOnlyList<GP.Interfaces.IVariable> variables, string setName, ref GP.Solver gpa, ref Dictionary<GP.Variable, GP.Variable> oldToNew)
        {
            setName += " - Dummy Variables";
            List<GP.Interfaces.IVariable> modelIVariables = new List<GP.Interfaces.IVariable>(variables.Count);

            for (int i = 0; i < variables.Count; i++)
            {
                GP.Interfaces.IVariable iVariable = variables[i];

                if (iVariable is GP.Variable variable)
                {
                    if (oldToNew.ContainsKey(variable))
                    {
                        GP.Variable modelVariable = oldToNew[variable];

                        modelIVariables.Add(modelVariable);
                    }
                    else
                    {
                        GP.Variable modelVariable = gpa.AddVariable(variable.ToArray());

                        oldToNew.Add(variable, modelVariable);
                        if(_setsOfVariables!.ContainsKey(setName)) { _setsOfVariables[setName].Add(modelVariable); }
                        else { _setsOfVariables.Add(setName, new List<GP.Variable>() { modelVariable }); }

                        modelIVariables.Add(modelVariable);

                        this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, "A variable is not contained in a set but is used in a constraint or an energy. It may be a dummy variable.");
                    }
                }
                else if (iVariable is GP.SubVariable subVariable)
                {
                    if (oldToNew.ContainsKey(subVariable.ReferenceVariable))
                    {
                        GP.Variable modelVariable = oldToNew[subVariable.ReferenceVariable];
                        GP.SubVariable modelSubVariable = new GP.SubVariable(modelVariable, subVariable.ReferenceIndices());

                        modelIVariables.Add(modelSubVariable);
                    }
                    else
                    {
                        throw new ArgumentException("A reference variable of a sub-variable is not contained in a set but is used in a constraint or an energy.");
                    }
                }
                else { throw new Exception($"A variable was not recognised as either a {typeof(GP.Variable)} or a {typeof(GP.SubVariable)}."); }
            }

            return modelIVariables;
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_SetOfVariables(), "Sets of Variables", "S", "Sets of variables used by the energies and constraints.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Parameters.Constraints.Param_Constraint(), "Constraints", "C", "Constraints for the model.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Parameters.Energies.Param_Energy(), "Energies", "E", "Energies for the model.", GH_Kernel.GH_ParamAccess.list);

            pManager.AddIntegerParameter("Number of Iterations", "I", "Number of iterations for the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddBooleanParameter("Run", "R", "Run the solver of the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);

            pManager[2].Optional = true;
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Models.Param_Model(), "Computed Model", "M", "The computed model using the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            bool run = false;


            //     -----     Get Inputs     -----     //

            if (!DA.GetData(4, ref run)) { return; }


            //     -----     Core     -----     //

            if (run)
            {
                ActualSolveInstance(DA);
                this.Message = null;
            }

            //     -----     Set Output     -----     //

            if (_setsOfVariables is null)
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, string.Empty);
                this.Message = "Not Runned";

                this.OnPingDocument().RequestAbortSolution();
                return;
            }
            else
            {
                Dictionary<string, List<GP.Constraint>>? _setsOfConstraints = new Dictionary<string, List<GP.Constraint>>();
                _setsOfConstraints.Add("Temporary", new List<GP.Constraint>());

                Dictionary<string, List<GP.Energy>>? _setsOfEnergies = new Dictionary<string, List<GP.Energy>>();
                _setsOfEnergies.Add("Temporary", new List<GP.Energy>());


                Types.Models.Gh_Model gh_Model = new Types.Models.Gh_Model(ref _setsOfVariables, ref _setsOfConstraints, ref _setsOfEnergies);

                DA.SetData(0, gh_Model);
            }
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{4378BC59-E97B-47F9-8ACE-1038ECFA2C66}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Solver;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;

        #endregion
    }
}
