using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Variables.Construct
{
    /// <summary>
    /// A grasshopper component creating a multi-dimensional <see cref="GP.Variable"/>.
    /// </summary>
    public class Comp_MultiDimensionalVariable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_MultiDimensionalVariable"/> class.
        /// </summary>
        public Comp_MultiDimensionalVariable()
          : base("m-Dimensional Variable", "mD Var.",
              "Defines a multi-dimensional variable for the Guided Projection algorithm.",
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
            pManager.AddNumberParameter("Component Values", "V", "Component values of the variable.", GH_Kernel.GH_ParamAccess.tree);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_SetOfVariables(), "Set", "S", "Set of variables for the Guided Projection Algorithm solver.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Variable", "V", "Newly created variable.", GH_Kernel.GH_ParamAccess.list);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = "";
            GH_Kernel.Data.GH_Structure<GH_Kernel.Types.GH_Number> gh_Structure = new GH_Kernel.Data.GH_Structure<GH_Kernel.Types.GH_Number>();


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; };
            if (!DA.GetDataTree(1, out gh_Structure)) { return; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(gh_Structure);


            //     -----     Core     -----    //

            List<GP.Variable> variables = new List<GP.Variable>(gh_Structure.PathCount);

            for (int i_Branch = 0; i_Branch < gh_Structure.PathCount; i_Branch++)
            {
                List<GH_Kernel.Types.GH_Number> gh_Branch = gh_Structure.Branches[i_Branch];

                List<double> components = new List<double>(gh_Branch.Count);
                for (int i_Item = 0; i_Item < gh_Branch.Count; i_Item++){ components.Add(gh_Branch[i_Item].Value); }

                GP.Variable variable = new GP.Variable(components);
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
        public override Guid ComponentGuid => new Guid("{EF4E7AEA-AD25-4CC9-8451-D7797FA98EF5}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Construct;

        #endregion
    }
}
