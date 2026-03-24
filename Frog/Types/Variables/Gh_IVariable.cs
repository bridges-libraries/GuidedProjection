using System;

using GH_Types = Grasshopper.Kernel.Types;


namespace Frog.Types.Variables
{
    /// <summary>
    /// General grasshopper type containing either a <see cref="Gh_Variable"/> or a <see cref="Gh_SubVariable"/>
    /// </summary>
    public class Gh_IVariable : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Grasshopper type containing or representing a <see cref="GP.Interfaces.IVariable"/>.
        /// </summary>
        protected GH_Types.IGH_Goo? m_value;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the contained grasshopper type.
        /// </summary>
        /// <remarks> It is either a <see cref="Gh_Variable"/> or a <see cref="Gh_SubVariable"/>. </remarks>
        public GH_Types.IGH_Goo? Value => m_value;


        /// <summary>
        /// Gets the size of the contained grasshopper type.
        /// </summary>
        public int Size { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class.
        /// </summary>
        public Gh_IVariable()
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            m_value = null;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from a <see cref="Gh_Variable"/>.
        /// </summary>
        /// <param name="gh_Variable"> Grasshopper variable. </param>
        public Gh_IVariable(Gh_Variable gh_Variable)
        {
            //     -----     Verifications     -----     //

            if (!gh_Variable.IsValid) { throw new ArgumentException("The variable to store is not valid."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            m_value = gh_Variable;

            // Initialise Properties
            Size = gh_Variable.Size;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from a <see cref="Gh_SubVariable"/>.
        /// </summary>
        /// <param name="gh_SubVariable"> Grasshopper subVariable. </param>
        public Gh_IVariable(Gh_SubVariable gh_SubVariable)
        {
            //     -----     Verifications     -----     //

            if (!gh_SubVariable.IsValid) { throw new ArgumentException("The subvariable to duplicate is not valid."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            m_value = gh_SubVariable;

            // Initialise Properties
            Size = gh_SubVariable.Size;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_IVariable" /> class from another <see cref="Gh_IVariable"/>.
        /// </summary>
        /// <param name="gh_IVariable"> <see cref="Gh_IVariable"/> to shallow copy. </param>
        public Gh_IVariable(Gh_IVariable gh_IVariable)
        {
            //     -----     Verifications     -----     //

            if (!gh_IVariable.IsValid) { throw new ArgumentException($"The {typeof(Gh_IVariable)} to duplicate is not valid."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            m_value = gh_IVariable.m_value;
        }


        #endregion


        #region Implement : GH_Types.IGH_Goo

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => m_value is not null && m_value.IsValid;

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if (m_value is null) { return "The contained type is null."; }
                    else { return m_value.IsValidWhyNot; }
                }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => $"Grasshopper type containing either a {typeof(Gh_Variable)} or a {typeof(Gh_SubVariable)}.";

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_IVariable);


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_IVariable(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            //     -----     Grasshopper Types     -----     //

            // Cast a Gh_Variable to a Gh_IVariable
            if (typeof(Gh_Variable).IsAssignableFrom(type))
            {
                Gh_Variable gh_Variable = (Gh_Variable)source;

                m_value = gh_Variable;
                Size = gh_Variable.Size;


                return true;
            }

            // Cast a Gh_SubVariable to a Gh_IVariable
            if (typeof(Gh_SubVariable).IsAssignableFrom(type))
            {
                Gh_SubVariable gh_SubVariable = (Gh_SubVariable)source;
                m_value = gh_SubVariable;
                Size = gh_SubVariable.Size;

                return true;
            }

            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{Q}(ref Q)"/>
        public bool CastTo<T>(out T? target)
        {
            target = default;

            //     -----     Otherwise     -----     //

            return false;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.ToString()"/>
        public override string ToString()
        {
            if (m_value is null) { return string.Empty; }
            else 
            {
                return m_value.ToString();
            }
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;


        /// <inheritdoc cref="GH_Types.IGH_Goo.EmitProxy()"/>
        public GH_Types.IGH_GooProxy? EmitProxy() => null;

        #endregion
    }
}
