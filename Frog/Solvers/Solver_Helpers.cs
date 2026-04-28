using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Solvers
{
    /// <summary>
    /// Dummy class for cloning <see cref="GP.Interfaces.IConstraintType"/>, which are not <see cref="GP.Abstracts.UpdatedConstraintType"/>.
    /// </summary>
    internal class CloneConstraintType : GP.Abstracts.ConstraintType
    {
        #region Constructors

        public CloneConstraintType(GP.Interfaces.IConstraintType constraintType)
        {
            LocalHi = constraintType.LocalHi;
            LocalBi = constraintType.LocalBi;
            Ci = constraintType.Ci;
        }

        #endregion
    }

    /// <summary>
    /// Dummy class for cloning <see cref="GP.Abstracts.UpdatedConstraintType"/>.
    /// </summary>
    internal class CloneUpdatedConstraintType : GP.Abstracts.UpdatedConstraintType
    {
        #region Fields

        GP.Abstracts.UpdatedConstraintType _constraintType;

        #endregion

        #region Constructors

        public CloneUpdatedConstraintType(GP.Abstracts.UpdatedConstraintType constraintType)
        {
            _constraintType = constraintType;

            LocalHi = constraintType.LocalHi;
            LocalBi = constraintType.LocalBi;
            Ci = constraintType.Ci;
        }

        #endregion

        #region Public Methods

        public override void ComputeMembers(IReadOnlyList<GP.Interfaces.IVariable> variables,
            out BRIDGES.Numerics.LinearAlgebra.Matrices.SparseMatrix<double>? localHi, out BRIDGES.Numerics.LinearAlgebra.Vectors.SparseVector<double>? localBi, out double ci)
        {
            _constraintType.ComputeMembers(variables, out localHi, out localBi, out ci);
        }

        #endregion
    }


    /// <summary>
    /// Composite control that shows a Label next to a TextBox.
    /// </summary>
    internal class LabeledTextBox : System.Windows.Forms.UserControl
    {
        #region Properties

        public System.Windows.Forms.Label Label { get; init; }

        public System.Windows.Forms.TextBox TextBox { get; init; }

        #endregion

        #region Constructors

        public LabeledTextBox(string label, string initialText)
        {
            // ----- Create sub‑controls -----
            Label = new System.Windows.Forms.Label
            {
                // Context
                Text = label,
                // Display
                Width = label.Length * 4,
                Height = 20,
                Location = new System.Drawing.Point(5, 2),
            };

            TextBox = new System.Windows.Forms.TextBox
            {
                // Context
                Text = initialText,
                // Display
                Width = 150,
                Height = Label.Height,
                Location = new System.Drawing.Point(Label.Right + 5, 0),
                // Feature
                AcceptsTab = false,
                Multiline = false,
                AcceptsReturn = false,
            };

            // ----- Add them to the UserControl -----

            Controls.Add(Label);
            Controls.Add(TextBox);

            // Display
            Height = Label.Height + 3;
            Width = TextBox.Right;
            BackColor = System.Drawing.Color.Transparent;
        }

        #endregion
    }
}
