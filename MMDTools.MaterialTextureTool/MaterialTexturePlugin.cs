using MMDTools.Common;
using Newtonsoft.Json;
using PEPlugin;
using PEPlugin.Form;
using PEPlugin.Pmd;
using PEPlugin.Pmx;
using PEPlugin.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MMDTools.MaterialTextureTool
{
    public class MaterialTexturePlugin : PEPluginClass
    {
        public IPEPluginHost host;
        public IPEBuilder builder;
        public IPEShortBuilder bd;
        public IPEConnector connect;
        public IPEXPmd pex;
        public IPXPmx PMX;
        public IPEPmd PMD;
        public IPEFormConnector Form;
        public IPEPMDViewConnector PMDView;

        #region IPEPlugin

        public override string Name => "Export all material texture filename as list";

        public override string Version => "1.0.0.1";

        public override string Description => "A tool that used for save material filename to a list";

        #endregion
        public override void Run(IPERunArgs args)
        {
            this.host = args.Host;
            this.builder = this.host.Builder;
            this.bd = this.host.Builder.SC;
            this.connect = this.host.Connector;
            this.pex = this.connect.Pmd.GetCurrentStateEx();
            this.PMD = this.connect.Pmd.GetCurrentState();
            this.PMX = this.connect.Pmx.GetCurrentState();
            this.Form = this.connect.Form;
            this.PMDView = this.connect.View.PMDView;

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
                saveFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                DialogResult result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var materials = PMX.Material;
                    List<MaterialInfo> materialList = new List<MaterialInfo>();
                    foreach (var m in materials)
                    {
                        materialList.Add(new MaterialInfo() { Name = m.Name, Texture = m.Tex });
                    }

                    SaveToFile(saveFileDialog.FileName, materialList);
                }
            }
        }

        /// <summary>
        /// Save material list to a json file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="materialList"></param>
        private void SaveToFile(string path, List<MaterialInfo> materialList)
        {
            // serialize JSON to a string and then write string to a file
            File.WriteAllText(path, JsonConvert.SerializeObject(materialList));

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, materialList);
            }
        }
    }
}
