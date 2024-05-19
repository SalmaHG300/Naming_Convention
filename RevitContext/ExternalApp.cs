using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;

namespace WPF_RevitAPI_04.RevitContext
{
    public class ExternalApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("Naming Convention");
            RibbonPanel Panal = application.CreateRibbonPanel("Naming Convention", "Assign");

            string path =Assembly.GetExecutingAssembly().Location;

            PushButtonData p1 = new PushButtonData("Btn1","Plan Creation",path, "WPF_RevitAPI_04.RevitContext.Commands.OpenWindowCommand");

            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/WPF_RevitAPI_04;component/Resources/favicon (1).png"));
            p1.LargeImage = pb1Image;

            PushButtonData p2 = new PushButtonData("Btn2", "Sheet Creation", path, "WPF_RevitAPI_04.RevitContext.CreateSheet");

            BitmapImage pb2Image = new BitmapImage(new Uri("pack://application:,,,/WPF_RevitAPI_04;component/Resources/favicon (2).png"));
            p2.LargeImage = pb2Image;


            Panal.AddItem(p1);
            Panal.AddItem(p2);            
           

            return Result.Succeeded;
        }
       
    }
}
