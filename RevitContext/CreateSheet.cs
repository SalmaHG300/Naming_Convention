using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_RevitAPI_04.RevitContext.Commands;
using WPF_RevitAPI_04.ViewModel;
using Autodesk.Revit.Attributes;

namespace WPF_RevitAPI_04.RevitContext
{
    [Transaction(TransactionMode.Manual)]
    public class CreateSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = OpenWindowCommand.doc;
            UIDocument UIDoc = commandData.Application.ActiveUIDocument;
            doc = UIDoc.Document;
            //MainUIViewModel mainUIViewModel = new MainUIViewModel();
            //mainUIViewModel.ExcuteOpenCommand();
            OpenFileDialog dialogRead = new OpenFileDialog();
            dialogRead.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dialogRead.ShowDialog();
            string ExcelName = "";
            ExcelName = dialogRead.FileName;

            if (string.IsNullOrEmpty(ExcelName))
            {
                TaskDialog.Show("Invalidation", "No Excel file selected.");
            }

            try
            {
                using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(ExcelName)))
                {
                    var tblock = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .WhereElementIsElementType().FirstOrDefault();

                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Sheet"];
                    int numRows = worksheet.Dimension.Rows;
                    for (int row = 2; row <= numRows; row++)
                    {
                        using (Transaction trans = new Transaction(doc, "Create Sheet"))
                        {
                            trans.Start();

                            ViewSheet vs = ViewSheet.Create(doc, tblock.Id);
                            string PJNO = worksheet.Cells[row, 1].Value as string;
                            double BuNO = (double)worksheet.Cells[row, 2].Value;
                            string Dep = worksheet.Cells[row, 3].Value as string;
                            string Docno = worksheet.Cells[row, 4].Value as string;
                            //double Docno = (double)worksheet.Cells[row, 4].Value ;
                            vs.Name = $"{PJNO}-{BuNO}-{Dep}-{Docno}";
                            trans.Commit();
                        }

                    }

                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
    }
}
