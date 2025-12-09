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
        #region Private Properties

        /// <summary>
        /// Result vector of the guided projection algorithm.
        /// </summary>
        private List<double>? X { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Solver"/> class.
        /// </summary>
        public Comp_Solver()
          : base("GPA Solver", "GPA", "Creates and solves the model using the Guided Projection Algorithm (GPA).",
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

            List<Types.Variables.Gh_IVariable> gh_Variables = new List<Types.Variables.Gh_IVariable>();
            List<Types.Constraints.Gh_Constraint> gh_Constraints = new List<Types.Constraints.Gh_Constraint>();
            List<Types.Energies.Gh_Energy> gh_Energies = new List<Types.Energies.Gh_Energy>();

            int iterationCount = 0;

            //     -----     Get Inputs     -----     //

            DA.GetDataList(0, gh_Variables);
            if (!DA.GetDataList(1, gh_Constraints)) { return; }
            DA.GetDataList(2, gh_Energies);

            if (!DA.GetData(3, ref iterationCount)) { return; }

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Constraints);

            if (gh_Constraints.Count == 0) { throw new ArgumentException("The input C cannot be empty."); }


            //     -----     Core     -----     //

            double epsilon = 1e-5;

            GP.Solver gpa = CreateModel(epsilon, iterationCount, gh_Variables, gh_Constraints, gh_Energies);

            gpa.InitialiseX();

            for (int i = 0; i < iterationCount; i++)
            {
                gpa.RunIteration();
            }

            //     -----     Set Persistent Datas     -----     //

            X = new List<double>(gpa.ComponentCount);
            for (int i = 0; i < gpa.ComponentCount; i++)
            {
                X.Add(gpa.X[i]);
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
        private GP.Solver CreateModel(double epsilon, int maxIteration, List<Types.Variables.Gh_IVariable> gh_Variables, List<Types.Constraints.Gh_Constraint> gh_Constraints, List<Types.Energies.Gh_Energy> gh_Energies)
        {
            GP.Solver gpa = new GP.Solver(epsilon, maxIteration);

            AddVariablesToModel(gh_Variables, ref gpa, out Dictionary<GP.Variable, GP.Variable> oldToNew);

            AddConstraintsToModel(gh_Constraints, ref gpa, ref oldToNew);
            AddEnergiesToModel(gh_Energies, ref gpa, ref oldToNew);

            return gpa;
        }


        /// <summary>
        /// Sorts and adds the input grasshopper variable to the model.
        /// </summary>
        /// <param name="gh_Variables"> The list of input grasshopper variables. </param>
        /// <param name="gpa"> The model to operate on. </param>
        /// <param name="oldToNew"> The dictionary translating the input variables into model variables. </param>
        /// <returns> A dictionary translating the input variables into model variables. </returns>
        private void AddVariablesToModel(List<Types.Variables.Gh_IVariable> gh_Variables, ref GP.Solver gpa, out Dictionary<GP.Variable, GP.Variable> oldToNew)
        {
            oldToNew = new Dictionary<GP.Variable, GP.Variable>();
            foreach (Types.Variables.Gh_IVariable gh_Variable in gh_Variables)
            {
                GP.Interfaces.IVariable iVariable = gh_Variable.Value;

                if (iVariable is not GP.Variable) { continue; }

                GP.Variable variable = (iVariable as GP.Variable)!;
                if (oldToNew.ContainsKey(variable))
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, "A variable is contained twice in the input V.");
                }
                else
                {
                    GP.Variable modelVariable = gpa.AddVariable(variable.ToArray());

                    oldToNew.Add(variable, modelVariable);
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
                List<GP.Interfaces.IVariable> modelIVariables = new List<GP.Interfaces.IVariable>(constraint.Variables.Count);
                for (int i = 0; i < constraint.Variables.Count; i++)
                {
                    GP.Interfaces.IVariable iVariable = constraint.Variables[i];

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
                            modelIVariables.Add(modelVariable);

                            this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, "A variable is not contained in the input V but is used in a constraint. It may be a dummy variable.");
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
                            GP.Variable modelVariable = gpa.AddVariable(subVariable.ReferenceVariable.ToArray());

                            oldToNew.Add(subVariable.ReferenceVariable, modelVariable);

                            GP.SubVariable modelSubVariable = new GP.SubVariable(modelVariable, subVariable.ReferenceIndices());

                            modelIVariables.Add(modelSubVariable);

                            this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, "A variable is not contained in the input V but is used in a constraint. It may be a dummy variable.");
                        }
                    }
                    else { throw new Exception($"A constraint's variable was not recognised as either a {typeof(GP.Variable)} or a {typeof(GP.SubVariable)}."); }
                }


                gpa.AddConstraint(cloneConstraintType, modelIVariables, constraint.Weight);
            }
        }

        /// <summary>
        /// Sorts and adds the input grasshopper energies to the model.
        /// </summary>
        /// <param name="gh_Constraints"> The list of input grasshopper energies. </param>
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

                List<GP.Interfaces.IVariable> modelIVariables = new List<GP.Interfaces.IVariable>(energy.Variables.Count);
                for (int i = 0; i < energy.Variables.Count; i++)
                {
                    GP.Interfaces.IVariable iVariable = energy.Variables[i];

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
                            modelIVariables.Add(modelVariable);

                            this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, "A variable is not contained in the input V but is used in an energy. It may be a dummy variable.");
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
                            GP.Variable modelVariable = gpa.AddVariable(subVariable.ReferenceVariable.ToArray());

                            oldToNew.Add(subVariable.ReferenceVariable, modelVariable);

                            GP.SubVariable modelSubVariable = new GP.SubVariable(modelVariable, subVariable.ReferenceIndices());

                            modelIVariables.Add(modelSubVariable);

                            this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Remark, "A variable is not contained in the input V but is used in an energy. It may be a dummy variable.");
                        }
                    }
                    else { throw new Exception($"An energy's variable was not recognised as either a {typeof(GP.Variable)} or a {typeof(GP.SubVariable)}."); }
                }


                gpa.AddEnergy(cloneEnergyType, modelIVariables, energy.Weight);
            }
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Variables", "V", "Variables used by the energies and constraints. \r\n The order with which the variables are given will be preserved.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Parameters.Constraints.Param_Constraint(), "Constraints", "C", "Constraints", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Parameters.Energies.Param_Energy(), "Energies", "E", "Energies", GH_Kernel.GH_ParamAccess.list);

            pManager.AddIntegerParameter("Number of Iterations", "I", "Number of iterations of the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddBooleanParameter("Run", "R", "Run the solver of the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Result", "X", "Result vector.", GH_Kernel.GH_ParamAccess.list);
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
            /*else
            {
                if (X is null) { this.Message = "To Run"; }
                else { this.Message = "To Update"; }
            }*/

            //     -----     Set Output     -----     //

            DA.SetDataList(0, X);
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
