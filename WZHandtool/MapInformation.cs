using reWZ;
using reWZ.WZProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WZHandtool
{
    public partial class MapInformation : Form
    {
        private SortedDictionary<int, MapLifeInformation> MapCache;
        private ItemData ItemData;
        private List<ImageInformation> ImageInformation;
        private bool Mob;

        public MapInformation(SortedDictionary<int, MapLifeInformation> pMapCache, ItemData pItemData, bool pMob)
        {
            InitializeComponent();
            MapCache = pMapCache;
            ItemData = pItemData;
            ImageInformation = new List<ImageInformation>();
            Mob = pMob;
        }

        private void MapInformation_OnLoad(object sender, EventArgs e)
        {
            Text = ItemData.Name;
            if (Program.OldVersion)
            {
                WZFile wz;
                wz = new WZFile(Program.DataLocation + @"\Data.wz", WZVariant.BMS, true);
                TreeNode node;
                foreach (WZObject obj in wz.MainDirectory.ResolvePath(
                    GetPath(ItemData.ID)))
                {
                    node = new TreeNode(obj.Name);
                    node.Name = obj.Name;
                    node.Tag = obj;
                    treeView1.Nodes.Add(node);
                    int index = treeView1.Nodes.IndexOfKey(obj.Name);
                    foreach (WZObject sub in obj)
                    {
                        node = new TreeNode(sub.Name);
                        node.Name = sub.Name;
                        node.Tag = sub;
                        treeView1.Nodes[index].Nodes.Add(node);
                        int subIndex = treeView1.Nodes[index].Nodes.IndexOfKey(sub.Name);
                        foreach (WZObject sub2 in sub)
                        {
                            node = new TreeNode(sub2.Name);
                            node.Name = sub2.Name;
                            node.Tag = sub2;
                            treeView1.Nodes[index].Nodes[subIndex].Nodes.Add(node);
                            int subIndex2 = treeView1.Nodes[index].Nodes[subIndex].Nodes.IndexOfKey(sub2.Name);
                            foreach (WZObject sub3 in sub2)
                            {
                                node = new TreeNode(sub3.Name);
                                node.Name = sub3.Name;
                                node.Tag = sub3;
                                treeView1.Nodes[index].Nodes[subIndex].Nodes[subIndex2].Nodes.Add(node);
                            }
                        }
                    }
                }
                foreach (int mapId in MapCache.Keys)
                    foreach (int lifeId in MapCache[mapId].Life.Keys)
                        if (lifeId == ItemData.ID)
                            listBox1.Items.Add(mapId + " - " + MapCache[mapId].Name);
                if (listBox1.Items.Count > 1)
                    listBox1.Items.RemoveAt(0);
            }
        }

        private void MapInformation_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (ImageInformation imgInfo in ImageInformation)
                imgInfo.Close();
        }

        private string GetPath(int pID)
        {
            string result = pID.ToString();
            while (result.Length < 7)
                result = "0" + result;
            string prefix = "Npc/";
            if (Mob)
                prefix = "Mob/";
            return prefix + result + ".img";
        }

        private void treeView1_OnSelectComplete(object sender, TreeViewEventArgs e)
        {
            WZObject info = (WZObject)e.Node.Tag;
            string value = string.Empty;
            if (info is WZStringProperty || info is WZUOLProperty)
                value = info.ValueOrDie<string>();
            if (info is WZInt32Property)
                value = info.ValueOrDie<Int32>().ToString();
            if (info is WZSingleProperty)
                value = info.ValueOrDie<float>().ToString();
            if (info is WZPointProperty)
                value = info.ValueOrDie<Point>().ToString();
            if (value == string.Empty)
                value = info.GetType().Name;
            richTextBox1.Text = value;
            if (info is WZSubProperty)
            {
                if (info.ChildCount < 1)
                    return;
                if (!info.HasChild("0"))
                    return;
                foreach (WZObject obj in info)
                {
                    int i;
                    if (int.TryParse(obj.Name, out i))
                        if (!(obj is WZCanvasProperty) && !(obj is WZUOLProperty))
                            return;
                }
                ImageInformation.Add(new ImageInformation(info.Path, true));
                ImageInformation.ElementAt<ImageInformation>(ImageInformation.Count - 1).Show();
            }
        }
    }
}
