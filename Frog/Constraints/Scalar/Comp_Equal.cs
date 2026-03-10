using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


namespace Frog.Constraints.Scalar
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Scalar.Equal"/>-based <see cref="GP.Energy"/>.
    /// </summary>
    public class Comp_Equal : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Equal"/> class.
        /// </summary>
        public Comp_Equal()
          : base("Equal", "Equal",
              "Constraint enforcing a vector variable to be equal to a fixed vector.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Constraints])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Scalar Variable", "V", "Variable representing the vector which should equal a target vector. ", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Target Vector", "T", "Components of the target vector.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddNumberParameter("Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item, 1.0);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_Cstr.Param_Constraint(), "Constraint", "C", "Constraint", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            GP.Interfaces.IVariable? variable = null;
            double target = 0d;

            double weight = 0d;

            //     -----     Get Inputs     -----     //

            if (!DA.GetData(0, ref variable)) { return; }
            if (!DA.GetData(1, ref target)) { return; }

            if (!DA.GetData(2, ref weight)) { return; }

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(variable, "The input V is null");
            if (variable.Size != 1) { throw new ArgumentException("The inputs V and T do not have the same length."); }


            //     -----     Core     -----     //

            GP.ConstraintTypes.Scalar.Equal constraintType = new GP.ConstraintTypes.Scalar.Equal(target);

            GP.Interfaces.IVariable[] variables = new GP.Interfaces.IVariable[1] { variable };

            GP.Constraint constraint = new GP.Constraint(constraintType, variables, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{7D10E61F-BD86-4DB7-A286-9266EE723ED5}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Scalar;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;

        #endregion
    }
}
