using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WZHandtool
{
    class DataProvider
    {
        public Dictionary<string, SortedDictionary<int, ItemData>> Cache;
        private string path = Application.StartupPath + @"\Data\";

        public DataProvider()
        {
            Cache = new Dictionary<string, SortedDictionary<int, ItemData>>();
        }

        public bool Load(string category)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (FileStream fs = new FileStream(path + category + ".bin", FileMode.Open))
                {
                    SortedDictionary<int, ItemData> ItemList = new SortedDictionary<int, ItemData>();
                    ItemData itemData;
                    using (BinaryReader Reader = new BinaryReader(fs))
                    {
                        while (Reader.BaseStream.Position + 14 < Reader.BaseStream.Length)
                        {
                            try
                            {
                                itemData = new ItemData();
                                itemData.ID = Reader.ReadInt32();
                                itemData.Icon = (Bitmap)Reader.ReadBytes(Reader.ReadInt32()).ToImage();
                                itemData.Name = Encoding.UTF32.GetString(Reader.ReadBytes(Reader.ReadInt16()));
                                itemData.Description = Encoding.UTF32.GetString(Reader.ReadBytes(Reader.ReadInt16()));
                                itemData.Information = Encoding.UTF32.GetString(Reader.ReadBytes(Reader.ReadInt16()));
                                ItemList.Add(itemData.ID, itemData);
                            }
                            catch { }
                        }
                        if (!Cache.ContainsKey(category))
                            Cache.Add(category, ItemList);
                        else
                            Cache[category] = ItemList;
                    }
                }
                return Cache[category].Count > 0;
            }
            catch { }
            return false;
        }

        public bool Save(string category)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                using (FileStream fs = new FileStream(path + category + ".bin", FileMode.Create))
                {
                    using (BinaryWriter Writer = new BinaryWriter(fs))
                    {
                        foreach (ItemData itemData in Cache[category].Values)
                        {
                            Writer.Write(itemData.ID);
                            Writer.WriteBitmap(itemData.Icon);
                            Writer.WriteString(itemData.Name);
                            Writer.WriteString(itemData.Description);
                            Writer.WriteString(itemData.Information);
                        }
                    }
                }
                return true;
            }
            catch { }
            return false;
        }
    }
}
