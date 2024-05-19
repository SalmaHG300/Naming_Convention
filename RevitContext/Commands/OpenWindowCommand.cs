using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_RevitAPI_04.View;

namespace WPF_RevitAPI_04.RevitContext.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class OpenWindowCommand : IExternalCommand
    {
        #region Properties
        public static Document doc { get; set; }

        #endregion

        #region Method
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UIDoc = commandData.Application.ActiveUIDocument;
            doc = UIDoc.Document;

            try
            {
                MainUI mainUI = new MainUI();
                mainUI.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex )
            {
                message = ex.Message;
                return Result.Failed;
                
            }

        }
        #endregion

    }
}
