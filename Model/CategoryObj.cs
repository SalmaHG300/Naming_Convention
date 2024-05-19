using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_RevitAPI_04.Model
{
    public class CategoryObj
    {
        #region Properties
        public string CategoryName { get; set; }
        public CategoryType CategoryType { get; set; }
        public ViewFamily ViewFamily { get; set; }
        public BuiltInCategory BuiltInCategory { get; set; }
        #endregion

        #region Method
        public override string ToString()
        {
            return CategoryName;
        }
        #endregion


    }
}
