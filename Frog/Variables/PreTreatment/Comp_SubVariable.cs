using Grasshopper.Kernel.Geometry.SpatialTrees;
using Grasshopper.Kernel.Types.Transforms;
using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Variables.PreTreatment
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.SubVariable"/>.
    /// </summary>
    public class Comp_SubVariable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_OneDimensionalVariable"/> class.
        /// </summary>
        public Comp_SubVariable()
          : base("Sub-Variable", "Sub Var.",
              "Defines a sub-variable from the another variable.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            /* Do Nothing */
        }

        #endregion

        #region MyRegion

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Parent Variable", "V", "Variable whose components are used to create the sub-variable.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddIntegerParameter("Component Indices", "I", "Indices of the component retained to create the sub-variable.", GH_Kernel.GH_ParamAccess.list);
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

            GP.Interfaces.IVariable? iVariable = null;

            List<int> indices = new List<int>();


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref iVariable)) { return; };
            if (!DA.GetDataList(1, indices)) { return; };

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(iVariable, "The input V is null");
            if (iVariable is not GP.Variable variable) { throw new ArgumentException("The input V is not an actual variable."); }

            if (indices.Count == 0) { return; }

            //     -----     Core     -----    //

            
            GP.SubVariable subVariable = new GP.SubVariable(variable, indices);

            //     -----     Set Output     -----     //

            DA.SetData(0, subVariable);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{C29EA300-9913-4BB0-8435-9F6DBADC945C}");

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
