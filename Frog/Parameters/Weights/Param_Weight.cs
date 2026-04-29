using System;
using System.Collections.Generic;

using GH_Gui = Grasshopper.GUI;
using GH_Kernel = Grasshopper.Kernel;

using Types_W = Frog.Types.Weights;


namespace Frog.Parameters.Weights
{
    /// <summary>
    /// Grasshopper parameter containing an <see cref="Types_W.Gh_Weight"/>.
    /// </summary>
    public class Param_Weight : GH_Kernel.GH_PersistentParam<Types_W.Gh_Weight>
    {
        #region Event Handlers

        //     -----     For Menu_CustomSingleValueItem     -----     //

        protected void Menu_SingleDoubleValueTextChanged(GH_Gui.GH_MenuTextBox sender, string text)
        {
            if (text.Length == 0)
            {
                sender.TextBoxItem.ForeColor = System.Drawing.SystemColors.WindowText;
                return;
            }

            double destination = 0.0;
            if (GH_Kernel.GH_Convert.ToDouble(text, out destination, GH_Kernel.GH_Conversion.Secondary))
            {
                sender.TextBoxItem.ForeColor = System.Drawing.SystemColors.WindowText;
            }
            else
            {
                sender.TextBoxItem.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void Menu_SingleDoubleValueKeyDown(GH_Gui.GH_MenuTextBox sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Return:
                    {
                        string text = sender.Text;
                        text = (sender.Text = GH_Kernel.Expressions.GH_ExpressionSyntaxWriter.RewriteForGraphicInterface(text));
                        e.Handled = true;
                        base.PersistentData.Clear();
                        if (GH_Kernel.GH_Convert.ToDouble(text, out double destination, GH_Kernel.GH_Conversion.Secondary))
                        {
                            base.PersistentData.Append(new Types_W.Gh_Weight(destination));
                        }

                        OnObjectChanged(GH_Kernel.GH_ObjectEventType.PersistentData);
                        if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift
                            || System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                        {
                            sender.CloseEntireMenuStructure();
                        }

                        ExpireSolution(recompute: true);
                        break;
                    }
                case System.Windows.Forms.Keys.Escape:
                    sender.CloseEntireMenuStructure();
                    break;
            }
        }


        //     -----     For Menu_CustomMultiValueItem     -----     //

        private void Menu_MultiValueKeyDown(GH_Gui.GH_MenuCustomControl sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Return:
                    if (sender.Control is GH_Gui.GH_MultilineGooEditor gH_MultilineGooEditor)
                    {
                        RecordPersistentDataEvent("Set Multiple Weights");
                        GH_Kernel.Data.GH_Structure<GH_Kernel.Types.GH_Number> tree_Number = gH_MultilineGooEditor.GetDataTree<GH_Kernel.Types.GH_Number>();

                        GH_Kernel.Data.GH_Structure<Types_W.Gh_Weight> tree_Weight = new GH_Kernel.Data.GH_Structure<Types_W.Gh_Weight>();
                        for (int i_Path = 0; i_Path < tree_Number.PathCount; i_Path++)
                        {
                            GH_Kernel.Data.GH_Path path = tree_Number.Paths[i_Path];
                            List<GH_Kernel.Types.GH_Number> branch_Number = tree_Number[path];
                            for (int i_Item = 0; i_Item < branch_Number.Count; i_Item++)
                            {
                                GH_Kernel.Types.GH_Number gh_Number = branch_Number[i_Item];
                                if (gh_Number is null) { tree_Weight.Append(new Types_W.Gh_Weight(), path); }
                                else { tree_Weight.Append(new Types_W.Gh_Weight(gh_Number.Value), path); }
                            }
                        }

                        PersistentData.Clear();
                        PersistentData.MergeStructure(tree_Weight);

                        OnObjectChanged(GH_Kernel.GH_ObjectEventType.PersistentData);
                        if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift
                            || System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
                        {
                            sender.CloseEntireMenuStructure();
                        }

                        ExpireSolution(recompute: true);
                    }

                    break;
                case System.Windows.Forms.Keys.Escape:
                    sender.CloseEntireMenuStructure();
                    break;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Param_Weight"/>.
        /// </summary>
        public Param_Weight()
          : base("Constraint Weight", "Weight", "Contains a collection of weights for the constraints",
                GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Parameters])
        {
            // Do Nothing
        }

        #endregion


        #region Override : GH_PersistentParam<.>

        /// <inheritdoc cref="GH_Kernel.GH_PersistentParam{T}.Prompt_Singular(ref T)"/>
        protected override GH_Kernel.GH_GetterResult Prompt_Singular(ref Types_W.Gh_Weight value)
        {
            return GH_Kernel.GH_GetterResult.cancel;
        }

        /// <inheritdoc cref="GH_Kernel.GH_PersistentParam{T}.Prompt_Plural(ref List{T})"/>
        protected override GH_Kernel.GH_GetterResult Prompt_Plural(ref List<Types_W.Gh_Weight> values)
        {

            return GH_Kernel.GH_GetterResult.cancel;
        }



        /// <summary>
        /// Create the item for the "Set Weight" menu.
        /// </summary>
        /// <remarks> This method was implemented copying <see cref="GH_Kernel.Parameters.Param_Number.Menu_CustomSingleValueItem"/>. </remarks>
        /// <returns> The item for the menu. </returns>
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            string text = "";
            if (base.PersistentDataCount == 1)
            {
                Types_W.Gh_Weight gh_Weight = base.PersistentData.get_FirstItem(filter_nulls: false);
                if (gh_Weight != null && gh_Weight.IsConstant)
                {
                    text = gh_Weight._constant!.Value.ToString();
                }
            }

            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem($"Set Weight");
            GH_Kernel.GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem.DropDown, text,
                Menu_SingleDoubleValueKeyDown, Menu_SingleDoubleValueTextChanged, enabled: true, 200, lockOnFocus: true);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Create the item for the "Set Multiple Weights" menu.
        /// </summary>
        /// <remarks> This method was implemented copying <see cref="GH_Kernel.GH_PersistentParam{T}.Menu_CreateMultilineTextEditItem"/>. </remarks>
        /// <returns> The item for the menu. </returns>
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            GH_Gui.GH_MultilineGooEditor gH_MultilineGooEditor = new GH_Gui.GH_MultilineGooEditor();
            gH_MultilineGooEditor.SetDataTree(PersistentData);

            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Set Multiple Weights");
            GH_Kernel.GH_DocumentObject.Menu_AppendCustomItem(toolStripMenuItem.DropDown, gH_MultilineGooEditor, Menu_MultiValueKeyDown);
            return toolStripMenuItem;
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{41729092-1971-49E0-8D27-B0FC566F7CB2}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Weights;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Display.ParameterAttributes(this);
        }

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
