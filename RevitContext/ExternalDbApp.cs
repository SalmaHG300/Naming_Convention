using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_RevitAPI_04.RevitContext
{
    public class ExternalDbApp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            application.DocumentChanged += Application_DocumentChanged;

            return ExternalDBApplicationResult.Succeeded;
        }

        private Dictionary<ElementId, string> originalSheetNames = new Dictionary<ElementId, string>();

        private void Application_DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            //ElementCategoryFilter ecf = new ElementCategoryFilter(BuiltInCategory.OST_Views);

            //ElementCategoryFilter ec = new ElementCategoryFilter(BuiltInCategory.OST_Sheets);

            //ElementId eId = e.GetModifiedElementIds(ecf).FirstOrDefault();



            //string transName = e.GetTransactionNames().FirstOrDefault();

            //if (eId != null)
            //{

            //    TaskDialog.Show("element modified", eId.ToString() + " " + transName);

            //}
            // Check if any elements have been modified
            if (e.GetModifiedElementIds().Count > 0)
            {
                Document doc = e.GetDocument();

                // Check if any modified elements belong to the BuiltInCategory.OST_Views category
                ElementCategoryFilter viewFilter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                IEnumerable<ElementId> modifiedViews = e.GetModifiedElementIds(viewFilter);
                if (modifiedViews.Any())
                {
                    // Display a message for each modified view
                    foreach (ElementId viewId in modifiedViews)
                    {
                        Element modifiedElement = doc.GetElement(viewId);
                        if (modifiedElement is ViewSheet)
                        {
                            ViewSheet modifiedSheet = modifiedElement as ViewSheet;
                            TaskDialog.Show("Sheet Modified", $"Sheet Name: {modifiedSheet.Name} has been modified.");
                        }
                        else if (modifiedElement is ViewPlan)
                        {
                            ViewPlan modifiedPlan = modifiedElement as ViewPlan;
                            TaskDialog.Show("View Plan Modified", $"View Plan Name: {modifiedPlan.Name} has been modified.");
                        }
                    }
                }

                // Check if any modified elements belong to the BuiltInCategory.OST_Levels category
                ElementCategoryFilter levelFilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
                IEnumerable<ElementId> modifiedLevels = e.GetModifiedElementIds(levelFilter);
                if (modifiedLevels.Any())
                {
                    // Display a message for each modified level
                    foreach (ElementId levelId in modifiedLevels)
                    {
                        Element modifiedElement = doc.GetElement(levelId);
                        if (modifiedElement is Level)
                        {
                            Level modifiedLevel = modifiedElement as Level;
                            TaskDialog.Show("Level Modified", $"Level Name: {modifiedLevel.Name} has been modified.");
                        }
                    }

                }


                ElementCategoryFilter sheetFilter = new ElementCategoryFilter(BuiltInCategory.OST_Sheets);
                IEnumerable<ElementId> modifiedSheets = e.GetModifiedElementIds(sheetFilter);

                foreach (ElementId sheetId in modifiedSheets)
                {
                    Element modifiedElement = doc.GetElement(sheetId);
                    if (modifiedElement is ViewSheet modifiedSheet)
                    {
                        string originalName;
                        if (originalSheetNames.ContainsKey(sheetId))
                        {
                            originalName = originalSheetNames[sheetId];
                        }
                        else
                        {
                            originalName = modifiedSheet.Name;
                            originalSheetNames.Add(sheetId, originalName);
                        }

                        if (originalName != modifiedSheet.Name)
                        {
                            TaskDialog.Show("Sheet Name Changed", $"Sheet Name: {originalName} has been changed to {modifiedSheet.Name}.");
                        }
                    }
                }
            }
        }
    }
}
