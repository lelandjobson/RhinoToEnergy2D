using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace ITL.Energy2DExporter
{
    public class E2DExport
    {
        public static double Timestep = 0.1;
        public static double SunAngle = 1.5707964;
        public static double SolarPowerDensity = 2000.0;
        public static int SolarRayCount = 24;
        public static double SolarRaySpeed = 0.1;
        public static int PhotonEmissionInterval = 24;
        public static double GravitationAcceleration = 9.8;
        public static double ThermophoreticCoefficient = 0.0;
        public static double ParticleDrag = 0.01;
        public static string ParticleHardness = "1.0E-6";
        public static double ZHeatDiffusitivity = 0.0;
        public static string ThermalExpansionCoefficient = "2.5E-4";
        public static double BuoyancyApproximation = 0;
        public static double BorderTemp_Upper = 0.0;
        public static double BorderTemp_Lower = 0.0;
        public static double BorderTemp_Left = 0.0;
        public static double BorderTemp_Right = 0.0;
        public static int MassFlow_Upper = 0;
        public static int MassFlow_Lower = 0;
        public static int MassFlow_Left = 0;
        public static int MassFlow_Right = 0;
        public static int GridSize = 10;
        public static bool GridSnap = true;

        public static string Header =
        $"<?xml version = \"1.0\" encoding=\"UTF-8\"?>" + "\n" +
        $"<state>" + "\n" +
        $"<links>" + "\n" +
        $"</links>" + "\n" +
        $"<model>" + "\n" +
        $"<timestep>{Timestep}</timestep>" + "\n" +
        $"<sun_angle>{SunAngle}</sun_angle>" + "\n" +
        $"<solar_power_density>{SolarPowerDensity}</solar_power_density>" + "\n" +
        $"<solar_ray_count>{SolarRayCount}</solar_ray_count>" + "\n" +
        $"<solar_ray_speed>{SolarRaySpeed}</solar_ray_speed>" + "\n" +
        $"<photon_emission_interval>{PhotonEmissionInterval}</photon_emission_interval>" + "\n" +
        $"<gravitational_acceleration>{GravitationAcceleration}</gravitational_acceleration>" + "\n" +
        $"<thermophoretic_coefficient>{ThermophoreticCoefficient}</thermophoretic_coefficient><particle_drag>{ParticleDrag}</particle_drag><particle_hardness>{ParticleHardness}</particle_hardness><z_heat_diffusivity>{ZHeatDiffusitivity}</z_heat_diffusivity><z_heat_diffusivity_only_for_fluid>false</z_heat_diffusivity_only_for_fluid><thermal_expansion_coefficient>{ThermalExpansionCoefficient}</thermal_expansion_coefficient>" + "\n" +
        $"<buoyancy_approximation>{BuoyancyApproximation}</buoyancy_approximation>" + "\n" +
        $"<boundary>" + "\n" +
        $"<temperature_at_border upper = \"{BorderTemp_Upper}\" lower=\"{BorderTemp_Lower}\" left=\"{BorderTemp_Left}\" right=\"{BorderTemp_Right}\"/>" + "\n" +
        $"<mass_flow_at_border upper = \"{MassFlow_Upper}\" lower=\"{MassFlow_Lower}\" left=\"{MassFlow_Left}\" right=\"{MassFlow_Right}\"/>" + "\n" +
        "</boundary>" + "\n" +
        "<structure>" + "\n";

        public static string Footer =

        "<environment>" + "\n" +
        "</environment>" + "\n" +
        "<sensor>" + "\n" +
        "</sensor>" + "\n" +
        "<controller>" + "\n" +
        "</controller>" + "\n" +
        "</model>" + "\n" +
        "<view>" + "\n" +
        "<grid>true</grid>" + "\n" +
        $"<snap_to_grid>{GridSnap}</snap_to_grid>" + "\n" +
        $"<grid_size>{GridSize}</grid_size>" + "\n" +
        "<perimeter_step_size>0.05</perimeter_step_size>" + "\n" +
        "<border_tickmarks>true</border_tickmarks>" + "\n" +
        "<color_palette_type>0</color_palette_type>" + "\n" +
        "<color_palette_x>0.0</color_palette_x><color_palette_y>0.0</color_palette_y><color_palette_w>0.0</color_palette_w><color_palette_h>0.0</color_palette_h><minimum_temperature>0.0</minimum_temperature>" + "\n" +
        "<maximum_temperature>40.0</maximum_temperature>" + "\n" +
        "<fan_rotation_speed_scale_factor>1.0</fan_rotation_speed_scale_factor><graph_xlabel>Time (hr)</graph_xlabel><graph_ylabel>Temperature (℃)</graph_ylabel></view>" + "\n" +
        "</state>";


        private HashSet<Curve> _parts = new HashSet<Curve>();

        public E2DExport()
        {
        }

        public void AddPart(Curve c) => _parts.Add(c);


        string CreatePolygonPart(Polyline pl)
        {
            string vertices = "";
            int count = pl.Count;
            foreach(var p in pl)
            {
                vertices += $"{p.X}, {p.Y},";
            }
            vertices.TrimEnd(',');

            string partTemplate =
                "<part>" + "\n" +
                $"<polygon count=\"{count}\" vertices=\"{vertices}\"/>" + "\n" +
                "<elasticity>1.0</elasticity>" + "\n" +
                "<thermal_conductivity>1.0</thermal_conductivity>" + "\n" +
                "<specific_heat>1300.0</specific_heat>" + "\n" +
                "<density>25.0</density>" + "\n" +
                "<transmission>0.0</transmission>" + "\n" +
                "<reflection>0.0</reflection>" + "\n" +
                "<scattering>false</scattering>" + "\n" +
                "<absorption>1.0</absorption>" + "\n" +
                "<emissivity>0.0</emissivity>" + "\n" +
                "<temperature>20.0</temperature>" + "\n" +
                "<constant_temperature>false</constant_temperature>" + "\n" +
                "</part>";

            return partTemplate;
        }

        string CreateBlobPart(NurbsCurve part)
        {
            string vertices = "";
            for (int i = 0; i < NurbsCurveSampling + 1; i++)
            {
                // Sample the curve.
                Point3d p = part.PointAtNormalizedLength((1.0 / NurbsCurveSampling) * i);
                vertices += $"{p.X}, {p.Y},";
            }
            vertices.TrimEnd(',');

            string partTemplate =
            "<part>" + "\n" +
            $"<blob count=\"{NurbsCurveSampling}\" vertices=\"{vertices}\"/>" + "\n" +
            "<elasticity>1.0</elasticity>" + "\n" +
            "<thermal_conductivity>1.0</thermal_conductivity>" + "\n" +
            "<specific_heat>1300.0</specific_heat>" + "\n" +
            "<density>25.0</density>" + "\n" +
            "<transmission>0.0</transmission>" + "\n" +
            "<reflection>0.0</reflection>" + "\n" +
            "<scattering>false</scattering>" + "\n" +
            "<absorption>1.0</absorption>" + "\n" +
            "<emissivity>0.0</emissivity>" + "\n" +
            "<temperature>20.0</temperature>" + "\n" +
            "<constant_temperature>false</constant_temperature>" + "\n" +
            "</part>";

            return partTemplate;
        }

        public const string OutputFileExtension = ".e2d";

        /// <summary>
        /// The amount of sample points to take from
        /// a nurbs curve to convert it.
        /// </summary>
        public static int NurbsCurveSampling = 50;

        public void WriteFile(string path)
        {
            // Build file
            string output = Header.Copy();

            foreach(var part in _parts)
            {
                if (part.IsPolyline())
                {
                    part.TryGetPolyline(out var pl);
                    output += CreatePolygonPart(pl);
                }
                else
                {
                    var nurbsPart = part.ToNurbsCurve();
                    output += CreateBlobPart(nurbsPart);
                }
            }

            output += Footer;

            // Get path if none is supplied
            if (String.IsNullOrEmpty(path))
            {
                var saveFileDialog = new Rhino.UI.SaveFileDialog();
                saveFileDialog.DefaultExt = OutputFileExtension;
                if (saveFileDialog.ShowSaveDialog())
                {
                    path = saveFileDialog.FileName;
                }
                else
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
            }

            // Write out output
            System.IO.File.WriteAllText(path, output);

            // Let the user know
            RhinoApp.WriteLine($"Export successful! Wrote export to path: \"{path}\"");
        }
    }
}
