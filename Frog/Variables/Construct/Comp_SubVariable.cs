using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_Var = Frog.Types.Variables;


namespace Frog.Variables.Construct
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.SubVariable"/>.
    /// </summary>
    public class Comp_SubVariable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_SubVariable"/> class.
        /// </summary>
        public Comp_SubVariable()
          : base("Sub-Variable", "Sub Var.",
              "Defines a sub-variable from an variable.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Parent Variable", "V", "Variable whose coordinates are used to create the sub-variable.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddIntegerParameter("Coordinate Indices", "I", "Indices of the coordinates retained to create the sub-variable.", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Sub-Variable", "V", "Newly created sub-variable.", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            Types_Var.IGH_GooVariable? gh_IVariable = null;
            List<int> indices = new List<int>();


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref gh_IVariable)) { return; }
            if (!DA.GetDataList(1, indices)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(gh_IVariable, "The input V is null");
            ArgumentNullException.ThrowIfNull(indices, "The input I is null");

            if (gh_IVariable is not Types_Var.Gh_Variable gh_Variable)
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error, "The input V is not a variable.");
                return;
            }
            if (indices.Count == 0)
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error, "The input I is empty.");
                return;
            }

            //     -----     Core     -----    //


            Types_Var.Gh_SubVariable gh_SubVariable = new Types_Var.Gh_SubVariable(gh_Variable, indices);

            //     -----     Set Output     -----     //

            DA.SetData(0, gh_SubVariable);
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
        public override Guid ComponentGuid => new Guid("{C29EA300-9913-4BB0-8435-9F6DBADC945C}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon
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
