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
            get { return "ITLEnergy2DExporterCommand"; }
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
                    if (c != null) { hasAtLeastOneObject = true; export.AddPart(c); }
                }
                if (!hasAtLeastOneObject) { goto cancelCmd; }

                // Write out the export
                export.WriteFile(null);

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
    }
}
