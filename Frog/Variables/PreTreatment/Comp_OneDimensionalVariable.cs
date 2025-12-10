using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Variables.PreTreatment
{
    /// <summary>
    /// A grasshopper component creating a one-dimensional <see cref="GP.Variable"/>.
    /// </summary>
    public class Comp_OneDimensionalVariable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_OneDimensionalVariable"/> class.
        /// </summary>
        public Comp_OneDimensionalVariable()
          : base("1-Dimensional Variable", "1D Var.",
              "Defines a one-dimensional variable for the Guided Projection algorithm.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the set of variables.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Numeric Values", "V", "Values of the variables.", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_SetOfVariables(), "Set", "S", "Set of variables for the Guided Projection Algorithm solver.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Variables", "V", "Newly created variables for the Constraints and Energies.", GH_Kernel.GH_ParamAccess.list);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = "";
            List<double> values = new List<double>();


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; };
            if (!DA.GetDataList(1, values)) { return; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(values);


            //     -----     Core     -----    //

            List<GP.Variable> variables = new List<GP.Variable>(values.Count);

            for (int i = 0; i < values.Count; i++)
            {
                GP.Variable variable = new GP.Variable(values[i]);
                variables.Add(variable);
            }


            //     -----     Set Output     -----     //

            Types.Variables.Gh_SetOfVariables set = new Types.Variables.Gh_SetOfVariables(name, variables);

            DA.SetData(0, set);
            DA.SetDataList(1, variables);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{45077A70-010E-4511-AD87-8F8578925A81}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.PreTreatment;

        #endregion
    }
}
