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
    public partial class MainForm : Form
    {
        private Dictionary<string, WZFile> DataFiles;
        private DataProvider Provider;
        private SortedDictionary<int, ItemData> ItemList;
        private bool Update = false;

        public MainForm()
        {
            DataFiles = new Dictionary<string, WZFile>();
            InitializeComponent();
        }

        private void MainForm_OnLoad(object sender, EventArgs e)
        {
            Load LoadForm = new Load();
            LoadForm.ShowDialog();
            Init();
        }

        private void MainForm_OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Update)
                return;
            DialogResult Result = MessageBox.Show(
                "Would you like to save the loaded data into binary format for faster loading?",
                "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (Result == DialogResult.Cancel)
                e.Cancel = true;
            else if (Result == DialogResult.Yes)
            {
                foreach (string category in Provider.Cache.Keys)
                    Provider.Save(category);
            }
        }

        private void Init()
        {
            radioButton1.Checked = true;
            Provider = new DataProvider();
            if (Program.OldVersion)
            {
                DataFiles.Add("Data", new WZFile(Program.DataLocation + @"\Data.wz", WZVariant.BMS, true));
                comboBox1.Items.RemoveAt(0); //Cash
                comboBox1.Items.RemoveAt(12); //TamingMob
            }
            else
            {
                DataFiles.Add("Item", new WZFile(Program.DataLocation + @"\Item.wz", WZVariant.BMS, true));
                DataFiles.Add("Character", new WZFile(Program.DataLocation + @"\Character.wz", WZVariant.BMS, true));
                DataFiles.Add("String", new WZFile(Program.DataLocation + @"\String.wz", WZVariant.BMS, true));
            }
            dataGridView1.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle;
            //TimedMessageBox.Show("Loading information, it might take a few seconds.", "Loading...", 3000);
            //Add into cache
            //ParseWZ("Map");
            //ParseWZ("Mob");
            //ParseWZ("NPC");
            //ParseWZ("Consume");
            //ParseWZ("Etc");
            //ParseWZ("Skill");
        }

        private void comboBox1_OnSelection(object sender, EventArgs e)
        {
            if (!ParseWZ(comboBox1.Text))
                return;
            dataGridView1.DataSource = ItemList.Values.ToList<ItemData>();
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                dataGridView1.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i].Height = 40;
            dataGridView1.Columns[0].Width = 60;
            for (int i = 0; i < ItemList.Values.Count; i++)
            {
                Bitmap icon = ItemList.Values.ToList<ItemData>()[i].Icon;
                if (icon == null)
                    continue;
                if (icon.Width > dataGridView1.Columns[1].Width)
                    dataGridView1.Columns[1].Width = icon.Width;
                if (icon.Height > dataGridView1.Rows[i].Height)
                    dataGridView1.Rows[i].Height = icon.Height;
            }
        }

        private void MainForm_OnResize(object sender, EventArgs e)
        {
            Point before = new Point(0, 49);
            Point after = dataGridView1.Location;
            Size size = dataGridView1.Size;
            dataGridView1.Size = new Size(size.Width + (after.X - before.X),
                size.Height + (after.Y - before.Y));
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[dataGridView1.Columns.Count - 2].Width =
                    (int)(dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width / 1.5);
            }
        }

        private DataGridViewHeaderBorderStyle ProperColumnHeadersBorderStyle
        {
            get
            {
                return (SystemFonts.MessageBoxFont.Name == "Segoe UI") ?
                DataGridViewHeaderBorderStyle.None :
                DataGridViewHeaderBorderStyle.Raised;
            }
        }

        private void fieldSearch_OnTextChanged(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
                return;
            dataGridView1.DataSource = ItemList.Values.ToList<ItemData>();
            List<int> toRemove = new List<int>();
            for (int i = ItemList.Count; i > 0; i--)
            {
                ItemData itemData = ItemList.Values.ElementAt<ItemData>(i - 1);
                try
                {
                    if (itemData.ID.ToString().Contains(textBox1.Text))
                        continue;
                    if (itemData.Name.ToLower().Contains(textBox1.Text.ToLower()))
                        continue;
                    if (itemData.Information.ToLower().Contains(textBox1.Text.ToLower()))
                        continue;
                    if (itemData.Description.ToLower().Contains(textBox1.Text.ToLower()))
                        continue;
                }
                catch
                {
                    continue;
                }
                toRemove.Add(i - 1);
            }
            List<ItemData> newData = ItemList.Values.ToList<ItemData>();
            foreach (int i in toRemove)
                newData.RemoveAt(i);
            dataGridView1.DataSource = newData;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                dataGridView1.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            for (int i = 0; i < newData.Count; i++)
            {
                Bitmap icon = newData[i].Icon;
                if (icon == null)
                    continue;
                if (icon.Width > dataGridView1.Columns[1].Width)
                    dataGridView1.Columns[1].Width = icon.Width;
                if (icon.Height > dataGridView1.Rows[i].Height)
                    dataGridView1.Rows[i].Height = icon.Height;
            }
        }

        private bool ParseWZ(string selectedCategory)
        {
            ItemList = new SortedDictionary<int, ItemData>();
            ItemData itemData = new ItemData();
            if (radioButton1.Checked)
            {
                if (Provider.Load(selectedCategory))
                {
                    ItemList = Provider.Cache[selectedCategory];
                    return true;
                }
            }
            for (int i = 0; i < (Program.OldVersion ? DataFiles["Data"].MainDirectory.ChildCount : DataFiles.Count); i++)
            {
                if (radioButton1.Checked)
                    break;
                WZObject node;
                if (Program.OldVersion)
                    node = DataFiles["Data"].MainDirectory.ToList<WZObject>()[i];
                else
                    node = DataFiles[DataFiles.Keys.ElementAt<string>(i)].MainDirectory;
                //if (Provider.Cache.ContainsKey(comboBox1.Text))
                //{
                //    ItemList = Provider.Cache[comboBox1.Text];
                //    break;
                //}
                if (GetWZName(node, i).Equals("Item")
                    && GetWZCategory(GetSelectedCategory(selectedCategory)).Equals("Item"))
                    foreach (WZObject inventory in node)
                    {
                        if (!inventory.Name.Equals(GetSelectedCategory(selectedCategory)))
                            continue;
                        foreach (WZObject category in inventory)
                        {
                            if (inventory.Name.Equals("Pet"))
                            {
                                ParseItemStats(category, itemData, inventory, ItemList);
                                continue;
                            }
                            foreach (WZObject item in category)
                                ParseItemStats(item, itemData, category, ItemList);
                        }
                    }
                else if (GetWZName(node, i).Equals("Character")
                    && GetWZCategory(GetSelectedCategory(selectedCategory)).Equals("Character"))
                    foreach (WZObject type in node)
                    {
                        if (type is WZImage)
                            continue;
                        if (type.Name.Equals(comboBox1.Text))
                            foreach (WZObject item in type)
                                ParseItemStats(item, itemData, type, ItemList);
                    }
                else if (GetWZName(node, i).Equals("Map")
                    && GetWZCategory(GetSelectedCategory(selectedCategory)).Equals("Map"))
                    foreach (WZObject type in node)
                    {
                        if (!type.Name.Equals("Map"))
                            continue;
                        foreach (WZObject category in type)
                        {
                            if (!category.Name.StartsWith("Map"))
                                continue;
                            foreach (WZObject map in category)
                            {
                                int id = int.Parse(map.Name.Substring(0, 9));
                                itemData = new ItemData();
                                itemData.ID = id;
                                itemData.Information = "";
                                foreach (WZObject stat in map)
                                {
                                    if (stat.Name.Equals("info"))
                                        foreach (WZObject info in stat)
                                        {
                                            string value = string.Empty;
                                            if (info is WZStringProperty)
                                                value = info.ValueOrDie<string>();
                                            if (info is WZInt32Property)
                                                value = info.ValueOrDie<Int32>().ToString();
                                            if (info is WZSingleProperty)
                                                value = info.ValueOrDie<float>().ToString();
                                            if (info is WZPointProperty)
                                                value = info.ValueOrDie<Point>().ToString();
                                            if (value == string.Empty)
                                                value = GetType(info);
                                            itemData.Information += "{" + info.Name + "=" + value + "} ";
                                        }
                                    else if (stat.Name.Equals("miniMap"))
                                        foreach (WZObject info in stat)
                                            if (info.Name.Equals("canvas"))
                                                itemData.Icon = ((WZCanvasProperty)info).Value;
                                }
                                if (itemData.Icon == null)
                                    itemData.Icon = new Bitmap(1, 1);
                                else
                                    itemData.Icon = new Bitmap(itemData.Icon,
                                        new Size(itemData.Icon.Width / 2, itemData.Icon.Height / 2));
                                ItemList.Add(id, itemData);
                            }
                        }
                    }
                else if (GetWZName(node, i).Equals("String"))
                    foreach (WZObject type in node)
                    {
                        if (type.Name.Equals("Item.img")
                            && GetWZCategory(GetSelectedCategory(selectedCategory)).Equals("Item"))
                            foreach (WZObject inventory in type)
                            {
                                if (!inventory.Name.Equals(GetStringCategory(GetSelectedCategory(selectedCategory))))
                                    continue;
                                foreach (WZObject item in inventory)
                                {
                                    foreach (WZObject stat in item)
                                    {
                                        int id;
                                        if (!int.TryParse(item.Name, out id))
                                            continue;
                                        if (!ItemList.ContainsKey(id))
                                            continue;
                                        itemData = ItemList[id];
                                        if (stat.Name.Equals("desc"))
                                            itemData.Description = stat.ValueOrDie<string>();
                                        if (stat.Name.Equals("name"))
                                            itemData.Name = stat.ValueOrDie<string>();
                                        ItemList[id] = itemData;
                                    }
                                }
                            }
                        else if (type.Name.Equals("Item.img")
                            && GetWZCategory(GetSelectedCategory(selectedCategory)).Equals("Character"))
                            foreach (WZObject inventory in type)
                            {
                                if (!inventory.Name.Equals("Eqp"))
                                    continue;
                                foreach (WZObject category in inventory)
                                {
                                    if (!category.Name.Equals(comboBox1.Text))
                                        continue;
                                    foreach (WZObject item in category)
                                    {
                                        foreach (WZObject stat in item)
                                        {
                                            int id;
                                            if (!int.TryParse(item.Name, out id))
                                                continue;
                                            if (!ItemList.ContainsKey(id))
                                                continue;
                                            itemData = ItemList[id];
                                            if (stat.Name.Equals("desc"))
                                                itemData.Description = stat.ValueOrDie<string>();
                                            if (stat.Name.Equals("name"))
                                                itemData.Name = stat.ValueOrDie<string>();
                                            ItemList[id] = itemData;
                                        }
                                    }
                                }
                            }
                        else if (type.Name.Equals("Map.img")
                            && GetWZCategory(GetSelectedCategory(selectedCategory)).Equals("Map"))
                            foreach (WZObject location in type)
                            {
                                foreach (WZObject map in location)
                                {
                                    int id;
                                    string streetName = string.Empty;
                                    string mapName = string.Empty;
                                    if (!int.TryParse(map.Name, out id))
                                        continue;
                                    if (!ItemList.ContainsKey(id))
                                        continue;
                                    foreach (WZObject stat in map)
                                    {
                                        itemData = ItemList[id];
                                        if (stat.Name.Equals("mapDesc"))
                                            itemData.Description = stat.ValueOrDie<string>();
                                        if (stat.Name.Equals("streetName"))
                                            streetName = stat.ValueOrDie<string>();
                                        else if (stat.Name.Equals("mapName"))
                                            mapName = stat.ValueOrDie<string>();
                                    }
                                    itemData.Name = streetName + " : " + mapName;
                                    ItemList[id] = itemData;
                                }
                            }
                    }
            }
            if (!Provider.Cache.ContainsKey(comboBox1.Text))
            {
                Provider.Cache.Add(comboBox1.Text, ItemList);
                Update = true;
            }
            return true;
        }

        private void ParseItemStats(WZObject item, ItemData itemData, WZObject parent, SortedDictionary<int, ItemData> list)
        {
            int id = int.Parse(item.Name.Substring(0, parent.Name.Equals("Pet") ? 7 : 8));
            if (list.ContainsKey(id))
                return;
            itemData = new ItemData();
            itemData.ID = id;
            itemData.Information = "";
            foreach (WZObject stat in item)
            {
                if (stat.Name.Equals("info"))
                    foreach (WZObject info in stat)
                    {
                        if (info.Name.Equals("icon") || info.Name.Equals("iconRaw"))
                        {
                            if (info is WZCanvasProperty && itemData.Icon == null)
                                itemData.Icon = ((WZCanvasProperty)info).Value;
                            else if (info is WZUOLProperty)
                                if (((WZUOLProperty)info).Value.Contains("../../"))
                                    itemData.Icon = ((WZCanvasProperty)parent.ResolvePath(
                                        ((WZUOLProperty)info).Value.Replace(@"../", ""))).Value;
                            continue;
                        }
                        string value = string.Empty;
                        if (info is WZStringProperty)
                            value = info.ValueOrDie<string>();
                        if (info is WZInt32Property)
                            value = info.ValueOrDie<Int32>().ToString();
                        if (info is WZPointProperty)
                            value = info.ValueOrDie<Point>().ToString();
                        if (value == string.Empty)
                            value = GetType(info);
                        itemData.Information += "{" + info.Name + "=" + value + "} ";
                    }
                else if (stat.Name.Equals("default"))
                    foreach (WZObject info in stat)
                    {
                        if (info.Name.Equals("hairOverHead"))
                            itemData.Icon = ((WZCanvasProperty)info).Value;
                        else if (info.Name.Equals("hair") && itemData.Icon == null)
                            itemData.Icon = ((WZCanvasProperty)info).Value;
                        else if (info.Name.Equals("face"))
                            itemData.Icon = ((WZCanvasProperty)info).Value;
                    }
            }
            list.Add(itemData.ID, itemData);
        }

        private string GetSelectedCategory(string category)
        {
            switch (category)
            {
                case "Cash":
                    return "Cash";
                case "Consume":
                    return "Consume";
                case "Weapon":
                    return "Equip";
                case "Cap":
                    return "Equip";
                case "Coat":
                    return "Equip";
                case "Longcoat":
                    return "Equip";
                case "Pants":
                    return "Equip";
                case "Shoes":
                    return "Equip";
                case "Glove":
                    return "Equip";
                case "Ring":
                    return "Equip";
                case "Cape":
                    return "Equip";
                case "Accessory":
                    return "Equip";
                case "Shield":
                    return "Equip";
                case "TamingMob":
                    return "Equip";
                case "Hair":
                    return "Equip";
                case "Face":
                    return "Equip";
                case "Map":
                    return "Map";
                case "Pet":
                    return "Pet";
                case "Install":
                    return "Install";
                case "Etc":
                    return "Etc";
                default:
                    return string.Empty;
            }
        }

        private string GetStringCategory(string category)
        {
            if (category.Length == 0)
                return string.Empty;
            if (category.Equals("Equip"))
                return "Eqp";
            return category.Substring(0, 3);
        }

        private string GetWZCategory(string category)
        {
            switch (category)
            {
                case "Cash":
                    return "Item";
                case "Consume":
                    return "Item";
                case "Equip":
                    return "Character";
                case "Pet":
                    return "Item";
                case "Install":
                    return "Item";
                case "Etc":
                    return "Item";
                case "Map":
                    return "Map";
                default:
                    return string.Empty;
            }
        }

        private string GetWZName(WZObject obj, int i)
        {
            if (Program.OldVersion)
                return obj.Name;
            return DataFiles.Keys.ElementAt<string>(i);
        }

        private string GetType(WZObject WZType)
        {
            if (WZType is WZAudioProperty)
                return "Audio";
            else if (WZType is WZCanvasProperty)
                return "Image";
            else if (WZType is WZUOLProperty)
                return "Path";
            else
                return WZType.Type.ToString();
        }
    }
}
