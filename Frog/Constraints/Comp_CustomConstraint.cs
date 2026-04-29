using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_V = Frog.Types.Variables;
using Types_C = Frog.Types.Constraints;
using Params_V = Frog.Parameters.Variables;
using Params_C = Frog.Parameters.Constraints;


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
            pManager.AddTextParameter("Name", "N", "Name of the constraint.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Params_V.Param_IVariable(), "Variables", "V", "Ordered list of variables for the custom constraint type.", GH_Kernel.GH_ParamAccess.list);
            pManager.AddParameter(new Params_C.Param_ConstraintType(), "Constraint Type", "Ct", "Custom constraint type", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Parameters.Weights.Param_Weight(), "Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item);

            // Set default value of the weight 
            Parameters.Weights.Param_Weight param = (Parameters.Weights.Param_Weight)pManager[3];
            param.PersistentData.Append(new Types.Weights.Gh_Weight(1.0));
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_C.Param_Constraint(), "Constraint", "C", "Newly created constraint", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = string.Empty;

            List<Types_V.IGH_GooVariable> gh_Variables = new List<Types_V.IGH_GooVariable>();
            Types_C.Gh_IConstraintType gh_ConstraintType = new Types_C.Gh_IConstraintType();

            double weight = 0d;


            //     -----     Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; }

            if (!DA.GetDataList(1, gh_Variables)) { return; }
            if (!DA.GetData(2, ref gh_ConstraintType)) { return; }

            if (!DA.GetData(3, ref weight)) { return; }

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Variables, "The input V is null");
            if (gh_Variables.Count == 0) { throw new ArgumentException("The input V is empty."); }


            //     -----     Core     -----     //

            // Create the constraint
            Types_C.Gh_Constraint gh_Constraint = new Types_C.Gh_Constraint(gh_Variables, gh_ConstraintType, weight, name);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
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
        public override Guid ComponentGuid => new Guid("{E870265E-B17A-43EF-B40E-66A0CF82E639}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Others;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
