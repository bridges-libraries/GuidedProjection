using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Types_Var = Frog.Types.Variables;


namespace Frog.Variables.Deconstruct
{
    /// <summary>
    /// A grasshopper component listing the coordinates of the <see cref="GP.Interfaces.IVariable"/>.
    /// </summary>
    public class Comp_DeconstructVariable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_DeconstructVariable"/> class.
        /// </summary>
        public Comp_DeconstructVariable()
          : base("Deconstruct Variable", "Dec. Var.",
              "Returns the coordinates of the variable or sub-variable.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Variable", "V", "Variable whose coordinates are listed.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Coordinate Values", "C", "Coordinates of the variable.", GH_Kernel.GH_ParamAccess.list);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            Types_Var.Gh_IVariable gh_IVariable = new Types_Var.Gh_IVariable();


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref gh_IVariable)) { return; }
            

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(gh_IVariable, "The input V is null");
            ArgumentNullException.ThrowIfNull(gh_IVariable.Value, "The input V is null");


            //     -----     Core     -----    //

            List<double> coordinates = new List<double>(gh_IVariable.Size);

            if (gh_IVariable.Value is Types_Var.Gh_Variable gh_Variable) 
            {
                for (int i = 0; i < gh_IVariable.Size; i++)
                {
                    coordinates.Add(gh_Variable[i]);
                }
            }
            else if (gh_IVariable.Value is Types_Var.Gh_SubVariable gh_SubVariable)
            {
                for (int i = 0; i < gh_IVariable.Size; i++)
                {
                    coordinates.Add(gh_SubVariable[i]);
                }
            }
            else { throw new InvalidCastException("The input V could not be casted to a variable or a subvariable"); }

            
            //     -----     Set Output     -----     //

            DA.SetDataList(0, coordinates);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{92B3D942-8F43-45E2-971D-338FAA32FB52}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon
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
