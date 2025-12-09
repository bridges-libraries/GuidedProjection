using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Variables.PreTreatment
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
            pManager.AddNumberParameter("Component Values", "V", "Component values of the variable.", GH_Kernel.GH_ParamAccess.list);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Variable", "V", "Newly created variable.", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            List<double> components = new List<double>();


            //     -----      Get Inputs     -----     //

            if (!DA.GetDataList(0, components)) { return; };


            //     -----     Core     -----    //

            GP.Variable variable = new GP.Variable(components);


            //     -----     Set Output     -----     //

            DA.SetData(0, variable);
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
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.PreTreatment;

        #endregion
    }
}
