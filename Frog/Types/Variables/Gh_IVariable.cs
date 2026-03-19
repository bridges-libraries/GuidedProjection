using Grasshopper.GUI;
using System;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Class defining a grasshopper type which contains a <see cref="GP.Interfaces.IVariable"/>, enhanced with additional information.
    /// </summary>
    public class Gh_IVariable : GH_Types.GH_Goo<GP.Interfaces.IVariable>
    {
        #region Properties

        /// <summary>
        /// Gets the set name of this Gh_IVariable.
        /// </summary>
        public string SetName { get; init; }

        /// <summary>
        /// Gets the index of this Gh_IVariable in the set.
        /// </summary>
        /// <remarks> If the index is equal to -1, there is two possibilities :
        /// <list type="number">
        ///     <item>
        ///         <description> The contained <see cref="GP.Interfaces.IVariable"/> is a SubVariable. </description>
        ///     </item>
        ///     <item>
        ///         <description> The contained <see cref="GP.Interfaces.IVariable"/> is a Variable but the indexing could not be achieved. </description>
        ///     </item>
        ///     <item>
        ///         <description> The contained <see cref="GP.Interfaces.IVariable"/> is null. </description>
        ///     </item>
        /// </list>either</remarks>
        public int IndexInSet { get; init; }


        /// <summary>
        /// Gets the Rhino data type to which the Gh_IVariable corresponds.
        /// </summary>
        public RhinoDataType DataType { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class.
        /// </summary>
        public Gh_IVariable()
        {
            // Initialise Properties
            SetName = string.Empty;
            IndexInSet = -1;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from a <see cref="GP.Interfaces.IVariable"/>.
        /// </summary>
        /// <param name="iVariable"> <see cref="GP.Interfaces.IVariable"/> to contain in the Gh_IVariable. </param>
        /// <param name="name"> Name of the set to which the Gh_IVariable belongs. </param>
        /// <param name="index"> Index of the Gh_IVariable in the set. </param>
        /// <param name="type"> Gets the Rhino data type to which the IVariable corresponds.</param>
        public Gh_IVariable(ref GP.Interfaces.IVariable iVariable, string name, int index, RhinoDataType type)
        {
            //     -----     Verifications
            if (type == RhinoDataType.Point & iVariable.Size != 3 ) { throw new ArgumentException("The size of the point variable is not equal three"); }

            // Initialise Fields
            m_value = iVariable;

            // Initialise Properties
            SetName = name;
            IndexInSet = index;

            DataType = type;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from another <see cref="Gh_IVariable"/>.
        /// </summary>
        /// <param name="gh_IVariable"> <see cref="Gh_IVariable"/> to shallow copy. </param>
        public Gh_IVariable(Gh_IVariable gh_IVariable)
        {
            // Initialise Fields
            Value = gh_IVariable.Value;

            // Initialise Properties
            SetName = gh_IVariable.SetName;
            IndexInSet = gh_IVariable.IndexInSet;

            DataType = gh_IVariable.DataType;
        }

        #endregion


        #region Override : GH_Goo<.>

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid => Value is not null && Value.Size != 0;

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValidWhyNot"/>
        public override string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if (Value is null) { return "The contained IVariable is null."; }
                    else if (Value.Size == 0) { return "The containe IVariable is empty."; }
                    else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                }
            }
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeDescription"/>
        public override string TypeDescription => $"Grasshopper type containing a {typeof(GP.Interfaces.IVariable)}.";

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeName"/>
        public override string TypeName => nameof(Gh_IVariable);


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.Duplicate"/>
        public override GH_Types.IGH_Goo Duplicate() => new Gh_IVariable(this);


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastFrom(object)"/>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {
            //     -----     Grasshopper Objects     -----     //

            // Casts a Gh_IVariable to a Rhino.Geometry.Point3d
            if (DataType == RhinoDataType.Point & typeof(T).IsAssignableFrom(typeof(GH_Types.GH_Point)) )
            {
                Rhino.Geometry.Point3d point = new Rhino.Geometry.Point3d(Value[0], Value[1], Value[2]);
                GH_Types.GH_Point gh_Point = new GH_Types.GH_Point(point);
                target = (T)(object)gh_Point;

                return true;
            }

            //     -----     Rhino Objects     -----     //

            // Casts a Gh_IVariable to a Rhino.Geometry.Point3d
            if (typeof(T).IsAssignableFrom(typeof(Rhino.Geometry.Point3d)) & DataType == RhinoDataType.Point)
            {
                Rhino.Geometry.Point3d point = new Rhino.Geometry.Point3d(Value[0], Value[1], Value[2]);
                target = (T)(object)point;

                return true;
            }

            //     -----     Otherwise     -----     //

            return false;
        }

        #endregion

        #region Override : Object

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                string text = "";

                if (Value is GP.Variable) { text += "Variable"; }
                else if (Value is GP.SubVariable) { text += "Sub-Variable"; }
                else { text += "IVariable"; }

                text += ": [" + Value[0];
                for (int i = 1; i < Value.Size; i++)
                {
                    text += ", " + Value[i];
                }
                text += "]";

                return text;
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion
    }
}
