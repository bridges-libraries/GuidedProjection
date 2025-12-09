using System;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Params_Cstr = Frog.Parameters.Constraints;
using Params_Var = Frog.Parameters.Variables;


namespace Frog.Constraints.Vector
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.VectorLength"/>-based <see cref="GP.Constraint"/>..
    /// </summary>
    public class Comp_VectorLength : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_VectorLength"/> class.
        /// </summary>
        public Comp_VectorLength()
          : base("Vector Length", "Length",
              "Create a Vector Length constraint for the Guided Projection Algorithm.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Constraints])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Vector Variable", "V", "Vector-like variable whose length should equal a specified target length", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Target Length", "L", "Target length of the vector variable.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddNumberParameter("Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item, 1.0);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_Cstr.Param_Constraint(), "Constraint", "C", "Vector Length Constraint", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            GP.Interfaces.IVariable? variable = null;
            double length = 0d;

            double weight = 0d;

            //     -----     Get Inputs     -----     //

            if (!DA.GetData(0, ref variable)) { return; };
            if (!DA.GetData(1, ref length)) { return; };

            if (!DA.GetData(2, ref weight)) { weight = 1d; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(variable, "The input V is null");


            //     -----     Core     -----     //

            int dimension = variable.Size;
            GP.Interfaces.IVariable[] variables = new GP.Interfaces.IVariable[1] { variable };

            GP.ConstraintTypes.VectorLength constraintType = new GP.ConstraintTypes.VectorLength(length, dimension);
            GP.Constraint constraint = new GP.Constraint(constraintType, variables, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{3925C384-0698-405D-9C77-ACD0A85AC720}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Vector;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;

        #endregion
    }
}
