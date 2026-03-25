using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_Var = Frog.Types.Variables;
using Types_Cstr = Frog.Types.Constraints;
using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


namespace Frog.Constraints
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.Constraint"/> from a custom <see cref="GP.Interfaces.IConstraintType"/>.
    /// </summary>
    public class Comp_CustomConstraint : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Equal"/> class.
        /// </summary>
        public Comp_CustomConstraint()
          : base("Custom Constraint", "Cstr",
              "Constraint defined from a custom constraint type.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Constraints])
        {
            // Do Nothing
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Variables", "V", "Ordered list of variables for the custom constraint type.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Params_Cstr.Param_ConstraintType(), "Constraint Type", "C", "Custom constraint type", GH_Kernel.GH_ParamAccess.item);

            pManager.AddNumberParameter("Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item, 1.0);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_Cstr.Param_Constraint(), "Constraint", "C", "Newly created constraint", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            List<Types_Var.IGH_GooVariable> gh_Variables = new List<Types_Var.IGH_GooVariable>();
            Types_Cstr.Gh_IConstraintType gh_ConstraintType = new Types_Cstr.Gh_IConstraintType();

            double weight = 0d;


            //     -----     Get Inputs     -----     //

            if (!DA.GetDataList(0, gh_Variables)) { return; }
            if (!DA.GetData(1, ref gh_ConstraintType)) { return; }

            if (!DA.GetData(2, ref weight)) { return; }

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Variables, "The input V is null");
            if (gh_Variables.Count == 0) { throw new ArgumentException("The input V is empty."); }


            //     -----     Core     -----     //

            // Create the constraint
            Types_Cstr.Gh_Constraint gh_Constraint = new Types_Cstr.Gh_Constraint(gh_Variables, gh_ConstraintType, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{E870265E-B17A-43EF-B40E-66A0CF82E639}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Others;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
