using Frog.Types.Variables;
using System;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Parameters.Variables
{
    /// <summary>
    /// A <see cref="Types.Variables.Gh_Variable"/> grasshopper parameter.
    /// </summary>
    public class Param_IVariable : GH_Kernel.GH_Param<IGH_GooVariable>
    {
        #region Properties

        /// <inheritdoc cref="GH_Kernel.IGH_PreviewObject.Hidden"/>
        public bool Hidden => true;

        /// <inheritdoc cref="GH_Kernel.IGH_PreviewObject.IsPreviewCapable"/>
        public bool IsPreviewCapable => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Param_IVariable"/>.
        /// </summary>
        public Param_IVariable()
          : base("Variable", "Variable", "Contains a collection of variables or sub-variables for the constraints and energies",
                GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Parameters], GH_Kernel.GH_ParamAccess.item)
        {
            // Do Nothing
        }

        #endregion


        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{00AE904F-69E5-4CEE-AD27-2A5F79F5D724}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Variables;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion

        #region Override : GH_ActiveObject

        /// <inheritdoc cref="GH_Kernel.GH_ActiveObject.Locked"/>
        public override bool Locked
        {
            get { return base.Locked; }
            set
            {
                if (base.Locked != value)
                {
                    base.Locked = value;
                }
            }
        }

        #endregion
    }
}
