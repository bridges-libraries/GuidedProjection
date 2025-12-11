using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Variables.Deconstruct
{
    /// <summary>
    /// A grasshopper component creating a multi-dimensional <see cref="GP.Variable"/>.
    /// </summary>
    public class Comp_FindSetOfVariables : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_FindSetOfVariables"/> class.
        /// </summary>
        public Comp_FindSetOfVariables()
          : base("Find Set", "Find", "Retrieves the variables of a set.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the set of variables to retrieve.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Parameters.Models.Param_Model(), "Computed Model", "M", "Computed model obtained with the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Variable's Components", "V", "Components of the variables contained in Set.", GH_Kernel.GH_ParamAccess.tree);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = "";
            Types.Models.Gh_Model gh_Model = new Types.Models.Gh_Model();

            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; };
            if (!DA.GetData(1, ref gh_Model)) { return; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(gh_Model);


            //     -----     Core     -----    //

            Grasshopper.DataTree<double> tree = new Grasshopper.DataTree<double>();

            if (gh_Model.SetsOfVariables is not null && 
                gh_Model.SetsOfVariables.TryGetValue(name, out List<GP.Variable>? set))
            {
                for (int i_Variable = 0; i_Variable < set.Count; i_Variable++)
                {
                    GP.Variable variable = set[i_Variable];

                    tree.AddRange(variable.ToArray(), new GH_Kernel.Data.GH_Path(i_Variable));
                }
            }
            else { AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, "No set with the specified name was found in the model."); }

            //     -----     Set Output     -----     //

            DA.SetDataTree(0, tree);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{48082F7A-C36C-431C-88C3-6A52008FCEE5}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Deconstruct;

        #endregion
    }
}
