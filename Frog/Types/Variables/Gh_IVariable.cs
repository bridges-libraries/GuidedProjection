using System;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Class defining a grasshopper type, representing a <see cref="GP.Interfaces.IVariable"/>.
    /// </summary>
    public class Gh_IVariable : GH_Types.GH_Goo<GP.Interfaces.IVariable>
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class.
        /// </summary>
        public Gh_IVariable() { /* Do Nothing */ }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from another <see cref="Gh_IVariable"/>.
        /// </summary>
        /// <param name="gh_Variable"> <see cref="Gh_IVariable"/> to shallow copy. </param>
        public Gh_IVariable(Gh_IVariable gh_Variable)
        {
            Value = gh_Variable.Value;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from a <see cref="GP.Interfaces.IVariable"/>.
        /// </summary>
        /// <param name="iVariable"> <see cref="GP.Interfaces.IVariable"/> for the energies and constraints. </param>
        public Gh_IVariable(ref GP.Interfaces.IVariable iVariable)
        {
            Value = iVariable;
        }

        #endregion


        #region Override : GH_Goo<.>

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid => Value is not null;

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

            //     -----     BRIDGES Objects     -----     //

            // Cast a GP.Interfaces.IVariable to a Gh_Variable
            if (typeof(GP.Interfaces.IVariable).IsAssignableFrom(type))
            {
                Value = (GP.Interfaces.IVariable)source;

                return true;
            }

            // Cast a GP.Variable to a Gh_Variable
            if (typeof(GP.Variable).IsAssignableFrom(type))
            {
                Value = (GP.Variable)source;

                return true;
            }

            // Cast a GP.SubVariable to a Gh_Variable
            if (typeof(GP.SubVariable).IsAssignableFrom(type))
            {
                Value = (GP.SubVariable)source;

                return true;
            }

            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {

            //     -----     BRIDGES Objects     -----     //

            // Casts a Gh_Variable to a GP.Interfaces.IVariable
            if (typeof(T).IsAssignableFrom(typeof(GP.Interfaces.IVariable)))
            {
                GP.Interfaces.IVariable iVariable = Value;
                target = (T)iVariable;

                return true;
            }

            // Casts a Gh_Variable to a GP.Variable
            if (typeof(T).IsAssignableFrom(typeof(GP.Variable)))
            {
                object variable = (GP.Variable)Value;
                target = (T)variable;

                return true;
            }

            // Casts a Gh_Variable to a GP.SubVariable
            if (typeof(T).IsAssignableFrom(typeof(GP.SubVariable)))
            {
                object subvariable = (GP.SubVariable)Value;
                target = (T)subvariable;

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
