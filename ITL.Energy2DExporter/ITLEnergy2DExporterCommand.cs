using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using System;
using System.Runtime.Remoting;

namespace ITL.Energy2DExporter
{

    public class ITLEnergy2DExporterCommand : Command
    {
        public ITLEnergy2DExporterCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ITLEnergy2DExporterCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "DFG_Export2E2D"; }
        }

        private bool ValidateDocParams()
        {
            if (RhinoDoc.ActiveDoc.ModelUnitSystem != UnitSystem.Meters)
            {
                RhinoApp.WriteLine("Please change your document unit system to meters in order to use the command.");
                return false;
            }
            return true;
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Rhino.DocObjects.ObjRef[] selectedObjects;

            if (!ValidateDocParams()) { return Result.Failure; }

            RhinoApp.WriteLine("Select closed curve geometry to export to Energy2D.");

            try
            {
                using (GetObject getObjectAction = new GetObject())
                {

                    var result = getObjectAction.GetMultiple(1, 2000);
                    if (result == GetResult.Nothing || result == GetResult.NoResult) { goto cancelCmd; }
                    selectedObjects = getObjectAction.Objects();
                }              
                if(selectedObjects == null || selectedObjects.Length == 0) { goto cancelCmd; }

                var export = new E2DExport();

                bool hasAtLeastOneObject = false;
                foreach (var o in selectedObjects)
                {
                    var c = o.Curve();
                    // Don't add open curves
                    if (!c.IsClosed) { continue; }
                    if (c != null) { hasAtLeastOneObject = true; export.AddPart(c); }
                }
                if (!hasAtLeastOneObject) { goto cancelCmd; }

                // Write out the export
                export.WriteFile(null);

                // Let the users know we care
                ShowRandomMsg();
                return Result.Success;

                cancelCmd:
                RhinoApp.WriteLine("Command cancelled.");
                return Result.Cancel;
            }
            catch(Exception e)
            {
                Logger.LogException(e);
                return Result.Failure;
            }
         
        }

        void ShowRandomMsg()
        {
            var rand = new Random();
            int intrnad = rand.Next(10);
            switch (intrnad)
            {
                case 1:
                    RhinoApp.WriteLine("The ITL wishes you a mediocre day");
                    break;
                case 2:
                    RhinoApp.WriteLine("The ITL wishes you an average day");
                    break;
                case 3:
                    RhinoApp.WriteLine("The ITL wishes you a special day");
                    break;
                case 4:
                    RhinoApp.WriteLine("The ITL wishes you a spectacular day");
                    break;
                case 5:
                    RhinoApp.WriteLine("The ITL wishes you a wonderful day");
                    break;
                case 6:
                    RhinoApp.WriteLine("The ITL wishes you a blessed day");
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    RhinoApp.WriteLine("The ITL wishes you a good day");
                    break;
            }
        }
    }
}
