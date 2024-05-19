using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPF_RevitAPI_04.Commands;
using WPF_RevitAPI_04.Model;
using WPF_RevitAPI_04.RevitContext.Commands;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using System.Runtime.Remoting.Messaging;
using OfficeOpenXml.DataValidation;
using System.Windows.Controls;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;


namespace WPF_RevitAPI_04.ViewModel
{
    public class MainUIViewModel :INotifyPropertyChanged
    {
        #region Field
        private CategoryObj _selectedCategoryObj;


        public string ExcelName ;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor
        public MainUIViewModel()
        {
            OpenCommand= new MyCommand(ExcuteOpenCommand);
            ApplyCommand = new MyCommand(ExcuteApplyCommand);
        }

       


        #endregion

        #region Properties
        public List<CategoryObj> CategoryObjList { get; set; } = new List<CategoryObj>()
        {
            //new CategoryObj(){CategoryName="Doors",BuiltInCategory=BuiltInCategory.OST_StructuralColumns},
           
            //new CategoryObj(){CategoryName="Window",BuiltInCategory=BuiltInCategory.OST_StructuralFraming},         

            new CategoryObj(){CategoryName="Floor Plan",ViewFamily = ViewFamily.FloorPlan},

            new CategoryObj(){CategoryName="Structural Plan",ViewFamily = ViewFamily.StructuralPlan},

            new CategoryObj(){CategoryName="Ceiling Plan",ViewFamily = ViewFamily.CeilingPlan},

            //new CategoryObj(){CategoryName="Sheet",ViewFamily = ViewFamily.Sheet}
        };

       
      
        public MyCommand OpenCommand { get; set; }
        public MyCommand ApplyCommand { get; set; }
        private string _excelBrowse;
        public string ExcelBrowse 
        {
            get
            {
                return _excelBrowse;

            }
            set
            {
                _excelBrowse = value;
                OnPropertyChanged();
            }

        }




        //this property shows the selected item from the combox with a data type categoryobj
        public CategoryObj SelectedCategoryObj
        {
            get
            {
                return _selectedCategoryObj;
                
            }
            set 
            { 
                _selectedCategoryObj = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Method
        public void OnPropertyChanged([CallerMemberName] string PropertName=null)
        {
            PropertyChanged.Invoke(this,new PropertyChangedEventArgs(PropertName));
        }

        public void ExcuteOpenCommand()
        {

            OpenFileDialog dialogRead = new OpenFileDialog();
            dialogRead.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dialogRead.ShowDialog();
            ExcelName = dialogRead.FileName;
            ExcelBrowse = ExcelName;
            if (string.IsNullOrEmpty(ExcelName))
            {
                TaskDialog.Show("Invalidation", "No Excel file selected.");
            }
            //else {
            //    //TaskDialog.Show("Message", "Excel has assigned successfully.");
            //    ExcuteApplyCommand();
            //}          
        }

        public void ExcuteApplyCommand()
        {
            Document doc = OpenWindowCommand.doc;
            var familyType = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
                      .FirstOrDefault(f => f.ViewFamily == SelectedCategoryObj.ViewFamily);
            if (familyType != null)
            {
                AssignedViewFamily(SelectedCategoryObj.ViewFamily,  familyType); 
                
            }

            else
            {
                TaskDialog.Show("Invalidation", $"Invalid view family: {familyType}");
            }              

        }         

        public void AssignedViewFamily(ViewFamily  ViewFamily, ViewFamilyType familyType )
        {
            Document doc = OpenWindowCommand.doc;
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(ExcelName)))
            {           
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[ViewFamily.ToString()];
                    int numRows = worksheet.Dimension.Rows;
                    for (int row = 2; row <= numRows; row++)
                    {
                        //if (ViewFamily.ToString() == "StructuralPlan" || ViewFamily.ToString() == "FloorPlan"|| ViewFamily.ToString() == "CeilingPlan")
                        //{
                            double ele = (double)worksheet.Cells[row, 3].Value;
                            double elevation = UnitUtils.ConvertToInternalUnits(ele, UnitTypeId.Millimeters);

                            string viewFamilyString = worksheet.Cells[row, 2].Value as string;
                            if (Enum.TryParse(viewFamilyString, out ViewFamily viewFamily))
                            {
                                string levelName = worksheet.Cells[row, 1].Value as string;

                                // Check if the level already exists
                                Level level = new FilteredElementCollector(doc)
                                    .OfClass(typeof(Level))
                                    .Cast<Level>()
                                    .FirstOrDefault(l => l.Name.Equals(levelName));


                                if (level == null)
                                {
                                    // Level doesn't exist, create a new one
                                    using (Transaction tran = new Transaction(doc, "Create Plan View"))
                                    {
                                        tran.Start();

                                        level = Level.Create(doc, elevation);

                                        level.Name = worksheet.Cells[row, 1].Value as string;


                                        //ViewPlan vp = ViewPlan.Create(doc, familyType.Id, level.Id);
                                        tran.Commit();
                                    }
                                }
                                else
                                {
                                    // Level already exists, update its elevation
                                    using (Transaction tran = new Transaction(doc, "Update Level"))
                                    {
                                        tran.Start();
                                        level.Elevation = elevation;
                                        //ViewPlan vp = ViewPlan.Create(doc, familyType.Id, level.Id);
                                        tran.Commit();
                                    }
                                }
                                // Check if the ViewPlan already exists
                                ViewPlan viewPlan = new FilteredElementCollector(doc)
                                    .OfClass(typeof(ViewPlan))
                                    .Cast<ViewPlan>()
                                    .FirstOrDefault(vp => vp.Name.Equals(levelName));

                                if (viewPlan == null)
                                {
                                    // ViewPlan doesn't exist, create a new one
                                    using (Transaction tran = new Transaction(doc, "Create ViewPlan"))
                                    {
                                        tran.Start();
                                        viewPlan = ViewPlan.Create(doc, familyType.Id, level.Id);
                                        viewPlan.Name = levelName;
                                        tran.Commit();
                                    }
                                }
                                else
                                {
                                    // ViewPlan already exists, create a new ViewPlan and delete the old one
                                    using (Transaction tran = new Transaction(doc, "Update ViewPlan"))
                                    {
                                        tran.Start();
                                        doc.Delete(viewPlan.Id);
                                        ViewPlan newViewPlan = ViewPlan.Create(doc, familyType.Id, level.Id);
                                        newViewPlan.Name = levelName;

                                        tran.Commit();
                                    }
                                }

                            }
                            else
                            {
                                TaskDialog.Show("Invalidation", $"Invalid view family: {viewFamilyString}");
                            }
                        //}
                        //    else
                        //    {}
                    }               

            } 
        }
            #endregion
    }

}
