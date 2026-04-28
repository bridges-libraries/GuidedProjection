using System;
using System.Collections.Generic;

using GH_Ker = Grasshopper.Kernel;


namespace Frog
{
    /// <summary>
    /// Name of the sub categories. The value is used to sort them of the grasshopper ribbon.
    /// </summary>
    public enum SubCategory : byte
    {
        Parameters = 0,
        Variables = 1,
        Constraints = 2,
        Solvers = 3,
        Results = 4
    }

    /// <summary>
    /// Information for the grasshopper library.
    /// </summary>
    public class GhLibraryInfo : GH_Ker.GH_AssemblyInfo
    {
        #region Properties

        /// <inheritdoc cref="GH_Ker.GH_AssemblyInfo.Name"/>
        public override string Name => CategoryName;

        /// <inheritdoc cref="GH_Ker.GH_AssemblyInfo.Description"/>
        public override string Description => "Grasshopper library for the Guided Projection Algorithm implemented in BRIDGES.";

        /// <inheritdoc cref="GH_Ker.GH_AssemblyInfo.Id"/>
        public override Guid Id => new Guid("{528EBBD6-6FF0-4F68-A1E2-C71F4D49FB19}");


        /// <inheritdoc cref="GH_Ker.GH_AssemblyInfo.AuthorName"/>
        public override string AuthorName => "ENPC - Build'In";

        /// <inheritdoc cref="GH_Ker.GH_AssemblyInfo.AuthorContact"/>
        public override string AuthorContact => "romain.mesnil@enpc.fr";

        #endregion
        
        #region Static Properties

        /// <summary>
        /// Nickname of this category.
        /// </summary>
        public static string CategoryName = "Frog";

        /// <summary>
        /// Name of the subcategories.
        /// </summary>
        /// <remarks> 
        /// Tab spaces are accounted for while sorting the subcategories but are trimmed before display (i.e. the more tab spaces, the earlier it will be). 
        /// Therefore, tab spaces are added to arrange the subcategories on the grasshopper ribbon.
        /// </remarks>
        public static Dictionary<SubCategory, string> SubCategoryName = InitialiseSubCategoryName();

        #endregion

        #region Static Methods

        /// <summary>
        /// Initialises <see cref="SubCategoryName"/> with the subcategory names with an appropriate number of spaces.
        /// </summary>
        /// <remarks> 
        /// Tab spaces are accounted for while sorting the subcategories but are trimmed before display (i.e. the more tab spaces, the earlier it will be). 
        /// Therefore, tab spaces are added to arrange the subcategories on the grasshopper ribbon.
        /// </remarks>
        /// <returns> The dictionary with the subcategory names. </returns>
        private static Dictionary<SubCategory, string> InitialiseSubCategoryName()
        {
            Dictionary<SubCategory, string> dictionary = new Dictionary<SubCategory, string>();

            byte max = 0;
            foreach (SubCategory subcatergory in (SubCategory[])Enum.GetValues(typeof(SubCategory)))
            {
                if (max < (byte)subcatergory) { max = (byte)subcatergory; }
            }
            foreach (SubCategory subcatergory in (SubCategory[])Enum.GetValues(typeof(SubCategory)))
            {
                dictionary.Add(subcatergory, new string('\t', max - (byte)subcatergory) + subcatergory.ToString());
            }

            return dictionary;
        }

        #endregion
    }
}
