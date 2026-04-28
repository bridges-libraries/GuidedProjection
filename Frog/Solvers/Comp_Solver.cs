using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Types_Var = Frog.Types.Variables;
using Types_Cstr = Frog.Types.Constraints;


namespace Frog.Solvers
{

    /// <summary>
    /// A grasshopper component creating a <see cref="GP.Solver"/> model.
    /// </summary>
    public class Comp_Solver : GH_Kernel.GH_Component
    {
        #region EventHandlers

        /// <summary>
        /// EventHandler for the text box defining the value of <see cref="Epsilon"/>.
        /// </summary>
        /// <param name="sender"> A <see cref="System.Windows.Forms.TextBox"/> for the value of <see cref="Epsilon"/>. </param>
        /// <param name="e"> An <see cref="EventArgs"/>. </param>
        private void TextBox_Epsilon_LostFocus(object? sender, EventArgs e)
        {
            System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)sender!;

            RecordUndoEvent("Epsilon");

            double newValue = Convert.ToDouble(textBox.Text);
            Epsilon = newValue;

            ExpireSolution(true);
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Sets of variables of the computed model.
        /// </summary>
        private Dictionary<string, GP.Variable[]> _setsOfVariables;

        /// <summary>
        /// Sets of constraints of the computed model.
        /// </summary>
        private Dictionary<string, List<GP.Constraint>> _setsOfConstraints;


        //     -----     GH_Component Utilities     -----     //

        private Dictionary<GH_Kernel.GH_RuntimeMessageLevel, IList<string>> _runtimeMessages = new Dictionary<GH_Kernel.GH_RuntimeMessageLevel, IList<string>>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the weight for the regulariser.
        /// </summary>
        public double Epsilon { get; private set; } = 1e-5;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Solver"/> class.
        /// </summary>
        public Comp_Solver()
          : base("GPA Solver", "GPA", "Creates and solves a model using the Guided Projection Algorithm (GPA).",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Solvers])
        {
            _setsOfVariables = new Dictionary<string, GP.Variable[]>();
            _setsOfConstraints = new Dictionary<string, List<GP.Constraint>>();
        }

        #endregion

        #region Private Methods

        //     -----     For the Solving     -----     //

        /// <summary>
        /// Contains the actual content for creating and solving the model with the Guided Projection Algorithm.
        /// </summary>
        /// <param name="DA"> Data Access object, used to retrieve the retrieve data from input parameter and assign data to output parameters. </param>
        /// <exception cref="ArgumentException"></exception>
        private bool ActualSolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            List<Types_Cstr.Gh_Constraint> gh_Constraints = new List<Types_Cstr.Gh_Constraint>();

            int iterationCount = 0;

            //     -----     Get Inputs     -----     //

            if (!DA.GetDataList(0, gh_Constraints)) { return false; }
            if (!DA.GetData(1, ref iterationCount)) { return false; }

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Constraints);
            if (gh_Constraints.Count == 0) { throw new ArgumentException("The input C is empty."); }


            //     -----     Core     -----     //

            bool isCreated = CreateGPAModel(Epsilon, iterationCount, gh_Constraints, out GP.Solver gpa);
            if (!isCreated) { return false; }


            gpa.InitialiseX();

            for (int i = 0; i < iterationCount; i++)
            {
                gpa.RunIteration();
            }

            //     -----     Finalise     -----     //

            return true;
        }


        /// <summary>
        /// Creates a Guided Projection Algorithm model.
        /// </summary>
        /// <returns> <see langword="true"/> if the model was created, <see langword="false"/> otherwise.  </returns>
        private bool CreateGPAModel(double epsilon, int maxIteration, List<Types_Cstr.Gh_Constraint> gh_Constraints,
            out GP.Solver model)
        {
            model = new GP.Solver(epsilon, maxIteration);

            Dictionary<string, int> vars_SetCount = new Dictionary<string, int>();
            Dictionary<Types_Var.Gh_Variable, GP.Variable> vars_GhToModel = new Dictionary<Types_Var.Gh_Variable, GP.Variable>();

            //      -----     Constraints

            bool cstrAdded = AddConstraints(model, gh_Constraints, vars_GhToModel, vars_SetCount);
            if (!cstrAdded) { return false; }


            //      -----     Store Variables in Sets

            bool varsStored = StoreModelVariables(vars_GhToModel, vars_SetCount);
            if (!varsStored) { return false; }

            return true;
        }


        /// <summary>
        /// Adds constraints to the model.
        /// </summary>
        /// <param name="model"> Guided Projection Algorithm model. </param>
        /// <param name="gh_Constraints"> List of constraint to add to the model. </param>
        /// <returns> <see langword="true"/> if the constraints were added, <see langword="false"/> otherwise. </returns>
        /// <exception cref="InvalidCastException"> 
        /// A <see cref="GP.Interfaces.IConstraintType"/> could not be casted into either <see cref="GP.Abstracts.ConstraintType"/>, nor a <see cref="GP.Abstracts.UpdatedConstraintType"/>. 
        /// </exception>
        private bool AddConstraints(GP.Solver model, List<Types_Cstr.Gh_Constraint> gh_Constraints,
            Dictionary<Types_Var.Gh_Variable, GP.Variable> vars_SwapGhToModel, Dictionary<string, int> vars_SetCount)
        {
            for (int i = 0; i < gh_Constraints.Count; i++)
            {
                Types_Cstr.Gh_Constraint gh_Constraint = gh_Constraints[i];


                //     -----     Variables

                IReadOnlyList<Types_Var.IGH_GooVariable> gh_IVariables = gh_Constraint.Variables;

                bool areReplaced = GhToModelVariables(model, gh_IVariables, vars_SwapGhToModel, vars_SetCount,
                    out List<GP.Interfaces.IVariable> modelIVars);

                if (!areReplaced)
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error,
                        $"The constraint at index {i} could not be added to the model because of its variables.");
                    return false;
                }


                //     -----     Constraint Type

                GP.Interfaces.IConstraintType iCstrType = gh_Constraint.ConstraintType.Value;

                GP.Interfaces.IConstraintType cloneCstrType;
                if (iCstrType is GP.Abstracts.UpdatedConstraintType updatedCstrType)
                {
                    cloneCstrType = new CloneUpdatedConstraintType(updatedCstrType);
                }
                else if (iCstrType is GP.Abstracts.ConstraintType cstrType)
                {
                    cloneCstrType = new CloneConstraintType(cstrType);
                }
                else
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error,
                        $"The constraint at index {i} could not be added to the model because of its constraint type.");
                    return false;
                }


                //     -----     Constraint

                GP.Constraint modelCstr = model.AddConstraint(cloneCstrType, modelIVars, gh_Constraint.Weight);

                if(_setsOfConstraints.TryGetValue(gh_Constraint.SetName, out List<GP.Constraint>? set))
                {
                    set.Add(modelCstr);
                }
                else { _setsOfConstraints.Add(gh_Constraint.SetName, new List<GP.Constraint>() { modelCstr }); }

            }

            return true;
        }


        /// <summary>
        /// Replaces the grasshopper variables by model variables. If a model variable doesn't exist, it is added. 
        /// </summary>
        /// <exception cref="InvalidCastException"> A <see cref="GP.Interfaces.IVariable"/> could not be casted into either <see cref="GP.Variable"/>, nor a <see cref="GP.SubVariable"/>. </exception>
        private bool GhToModelVariables(GP.Solver model, IReadOnlyList<Types_Var.IGH_GooVariable> gh_IVariables, Dictionary<Types_Var.Gh_Variable, GP.Variable> vars_SwapGhToModel, Dictionary<string, int> vars_SetCount,
            out List<GP.Interfaces.IVariable> modelIVars)
        {
            modelIVars = new List<GP.Interfaces.IVariable>(gh_IVariables.Count);

            for (int i = 0; i < gh_IVariables.Count; i++)
            {
                Types_Var.IGH_GooVariable gh_IVariable = gh_IVariables[i];

                if (gh_IVariable is Types_Var.Gh_Variable gh_Variable)
                {
                    if (vars_SwapGhToModel.TryGetValue(gh_Variable, out GP.Variable? modelVar)) { modelIVars.Add(modelVar); }
                    else
                    {
                        double[] coordinates = new double[gh_Variable.Size];
                        for (int i_Coord = 0; i_Coord < coordinates.Length; i_Coord++) { coordinates[i_Coord] = gh_Variable[i_Coord]; }

                        modelVar = model.AddVariable(coordinates);

                        vars_SwapGhToModel.Add(gh_Variable, modelVar);

                        if(!vars_SetCount.TryAdd(gh_Variable.SetName, 1)) { vars_SetCount[gh_Variable.SetName] += 1; }

                        modelIVars.Add(modelVar);
                    }
                }
                else if (gh_IVariable is Types_Var.Gh_SubVariable gh_SubVariable)
                {
                    Types_Var.Gh_Variable gh_ParentVariable = gh_SubVariable.ParentVariable;
                    if (vars_SwapGhToModel.TryGetValue(gh_ParentVariable, out GP.Variable? modelParentVar))
                    {
                        GP.SubVariable modelSubVar = new GP.SubVariable(modelParentVar, gh_SubVariable.RetaindeIndices);

                        modelIVars.Add(modelSubVar);
                    }
                    else
                    {
                        double[] coordinates = new double[gh_ParentVariable.Size];
                        for (int i_Coord = 0; i_Coord < coordinates.Length; i_Coord++) { coordinates[i_Coord] = gh_ParentVariable[i_Coord]; }

                        modelParentVar = model.AddVariable(coordinates);

                        vars_SwapGhToModel.Add(gh_ParentVariable, modelParentVar);

                        if ( !vars_SetCount.TryAdd(gh_ParentVariable.SetName, 1)) { vars_SetCount[gh_ParentVariable.SetName] += 1; }

                        GP.SubVariable modelSubVar = new GP.SubVariable(modelParentVar, gh_SubVariable.RetaindeIndices);

                        modelIVars.Add(modelSubVar);
                    }
                }
                else
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error,
                        $"A variable could not be casted into neither a {nameof(Types_Var.Gh_Variable)}, nor a {nameof(Types_Var.Gh_Variable)}");
                    return false;
                }
            }

            return true;
        }


        private bool StoreModelVariables(Dictionary<Types_Var.Gh_Variable, GP.Variable> vars_SwapGhToModel, Dictionary<string, int> vars_SetCount)
        {
            // Initialise the Set and Helpers

            _setsOfVariables = new Dictionary<string, GP.Variable[]>(vars_SetCount.Count);
            foreach (KeyValuePair<string, int> kvp in vars_SetCount)
            {
                _setsOfVariables.Add(kvp.Key, new GP.Variable[kvp.Value]);
            }

            Dictionary<string, bool?> isOrdered = new Dictionary<string, bool?>(vars_SetCount.Count);
            foreach (KeyValuePair<string, int> kvp in vars_SetCount)
            {
                isOrdered.Add(kvp.Key, null);
            }

            // Adds

            foreach (KeyValuePair<Types_Var.Gh_Variable, GP.Variable> kvp in vars_SwapGhToModel)
            {
                string name = kvp.Key.SetName;
                int index = kvp.Key.Index;

                if (index == -1)
                {
                    // Verifications
                    if (isOrdered[name] == null) { isOrdered[name] = false; }
                    else if ((isOrdered[name] == true))
                    {
                        this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error,
                            $"A set of variable is considered both ordered and unordered");
                        return false;
                    }

                    int length = _setsOfVariables[name].Length;

                    _setsOfVariables[name][length - vars_SetCount[name]] = kvp.Value;
                    vars_SetCount[name] -= 1;
                }
                else
                {
                    // Verifications
                    if (isOrdered[name] == null) { isOrdered[name] = true; }
                    else if ((isOrdered[name] == false))
                    {
                        this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error,
                            $"A set of variable is considered both ordered and unordered");
                        return false;
                    }

                    _setsOfVariables[name][index] = kvp.Value;
                }
            }

            return true;
        }


        //     -----     Other Methods     -----     //

        /// <summary>
        /// Clear the fields of this component before running a new model.
        /// </summary>
        private void ClearFields()
        {
            _setsOfVariables.Clear();
            _setsOfConstraints.Clear();
        }

        /// <summary>
        /// Saves the RuntimeMessages before exiting.
        /// </summary>
        private void SaveRuntimeMessages()
        {
            _runtimeMessages.Clear();

            foreach (GH_Kernel.GH_RuntimeMessageLevel level in Enum.GetValues(typeof(GH_Kernel.GH_RuntimeMessageLevel)))
            {
                _runtimeMessages.Add(level, this.RuntimeMessages(level));
            }
        }

        /// <summary>
        /// Repeats the saved RuntimeMessages, and clears the associated field.
        /// </summary>
        private void RepeatRuntimeMessages()
        {
            foreach (GH_Kernel.GH_RuntimeMessageLevel level in _runtimeMessages.Keys)
            {
                IList<string> messages = _runtimeMessages[level];
                foreach (string message in messages) { this.AddRuntimeMessage(level, message); }
            }

            _runtimeMessages = new Dictionary<GH_Kernel.GH_RuntimeMessageLevel, IList<string>>();
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Constraints.Param_Constraint(), "Constraints", "C", "Constraints for the model.", GH_Kernel.GH_ParamAccess.list);

            pManager.AddIntegerParameter("Number of Iterations", "I", "Number of iterations for the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddBooleanParameter("Run", "R", "Run the solver of the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);

            pManager[1].Optional = true;
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

            if (!DA.GetData(2, ref run)) { return; }


            //     -----     Core     -----     //

            if (run)
            {
                ClearFields();

                bool runned = ActualSolveInstance(DA);
                SaveRuntimeMessages();

                if (!runned) 
                {
                    this.Message = "To Update";
                    return; 
                }

                this.Message = " " + "Up-to-date" + " ";
            }
            if (!run)
            {
                // Case where "run" switches from true to false
                if (this.Message == (" " + "Up-to-date" + " "))
                {
                    RepeatRuntimeMessages();
                    this.Message = "Up-to-date";
                }
                // Case where "run" was already false
                else
                {
                    this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, "An input has changed. The model needs to be updated.");
                    this.Message = "To Update";
                }
            }


            //     -----     Set Output     -----     //

            if (_setsOfVariables is null)
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, string.Empty);
                this.Message = "Not Runned";

                return;
            }
            else
            {
                Types.Models.Gh_Model gh_Model = new Types.Models.Gh_Model(ref _setsOfVariables, ref _setsOfConstraints);

                DA.SetData(0, gh_Model);
            }
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown)"/>
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            // Append the item to the menu
            LabeledTextBox customItem = new LabeledTextBox("ε : ", Epsilon.ToString());

            Menu_AppendCustomItem(menu, customItem);

            // Manage the bejavior of the text box.
            customItem.TextBox.LostFocus += TextBox_Epsilon_LostFocus;

        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Display.ComponentAttributes(this);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{4378BC59-E97B-47F9-8ACE-1038ECFA2C66}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Solver;


        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Write(GH_IO.Serialization.GH_IWriter)"/>
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // Add custom fields.
            writer.SetDouble("Epsilon", Epsilon);

            // Call the base class implementation.
            return base.Write(writer);
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Read(GH_IO.Serialization.GH_IReader)"/>
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // Read custom fields.
            Epsilon = reader.GetDouble("Epsilon");

            // Call the base class implementation.
            return base.Read(reader);
        }

        #endregion
    }
}
