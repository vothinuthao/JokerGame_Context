using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Core.Manager
{
    public interface ISGConfigDataTable
    {
        void LoadFromAssetPath(string path, bool useAppDataPath = true);
        void Clear();
    }

    public class ConfigDataTable<TDataRecord> : ISGConfigDataTable where TDataRecord : class, new()
    {

        public bool IsLoaded;
        List<TDataRecord> records = new List<TDataRecord>();
        public class IndexField<TIndex> : Dictionary<TIndex, List<TDataRecord>> { };
        Dictionary<string, object> _rebuildIndex = new Dictionary<string, object>();

        List<FieldInfo> fields;

        public List<TDataRecord> Records
        {
            get { return records; }
        }

        public ConfigDataTable()
        {
            ReadFieldInfo();

        }

        void ReadFieldInfo()
        {
            Type type = typeof(TDataRecord);
            FieldInfo[] fieldArr = type.GetFields();
            fields = new List<FieldInfo>();
            foreach (FieldInfo filedInfo in fieldArr)
                if (!filedInfo.IsPrivate && !filedInfo.IsNotSerialized)
                    fields.Add(filedInfo);

        }

        public void LoadFromAssetPath(string path, bool useAppDataPath = true)
        {
            if (fields == null || fields.Count == 0)
                return;

            FileInfo theSourceFile = null;
            TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

            if (useAppDataPath)
                theSourceFile = new FileInfo(Application.dataPath + "/" + path + ".txt");
            else
                theSourceFile = new FileInfo(path);
            if (theSourceFile != null && theSourceFile.Exists)
            {
                reader = theSourceFile.OpenText();  // returns StreamReader
            }
            else
            {
                TextAsset puzdata = (TextAsset)Resources.Load(path, typeof(TextAsset));
                reader = new StringReader(puzdata.text);  // returns StringReader
            }
            if (reader == null)
            {
                Debug.Log("not found or not readable");
            }
            else
            {
                int line = 0;
                string txt = reader.ReadLine();// bo dong dau
                while (true)
                {
                    txt = reader.ReadLine();
                    line++;
                    if (txt == null || txt == "")
                    {
                        break;
                    }
                    TDataRecord record = new TDataRecord();//Activator.CreateInstance<TDataRecord>();

                    string[] columns = txt.Split('\t');

                    if (columns == null || columns.Length < fields.Count)
                    {
                        Debug.LogError("Load config " + path + " line " + line + " error " + columns.Length + "," + fields.Count);
                        continue;
                    }

                    int i = 0;
                    bool error = false;

                    foreach (FieldInfo field in fields)
                    {
                        object convert = ConvertData(columns[i], field.FieldType);
                        if (convert != null)
                            field.SetValue(record, convert);
                        //Ignore null Data
                        //else
                        //{
                        //                   error = true;
                        //                   break;
                        //               }

                        i++;
                    }
                    if (error)
                    {
                        Debug.LogError("Load config " + path + " line " + line + " error");
                        continue;
                    }
                    records.Add(record);

                }
                RebuildIndex();
                IsLoaded = true;
            }
        }

        public void LoadFromTextAsset(TextAsset textAsset)
        {
            TextReader reader = new StringReader(textAsset.text);  // returns StringReader
            if (reader == null)
            {
                Debug.Log("not found or not readable");
            }
            else
            {
                int line = 0;
                string txt = reader.ReadLine();// bo dong dau
                while (true)
                {
                    txt = reader.ReadLine();
                    line++;
                    if (txt == null) break;
                    TDataRecord record = new TDataRecord();//Activator.CreateInstance<TDataRecord>();

                    string[] columns = txt.Split('\t');

                    if (columns == null || columns.Length < fields.Count)
                    {
                        Debug.LogError("Load textasset " + textAsset.name + " line " + line + " error " + columns.Length + "," + fields.Count);
                        continue;
                    }

                    int i = 0;
                    bool error = false;

                    foreach (FieldInfo field in fields)
                    {
                        object convert = ConvertData(columns[i], field.FieldType);
                        if (convert != null)
                            field.SetValue(record, convert);
                        else
                        {
                            error = true;
                            break;
                        }

                        i++;
                    }
                    if (error)
                    {
                        Debug.LogError("Load textasset " + textAsset.name + " line " + line + " error");
                        continue;
                    }
                    records.Add(record);

                }
                RebuildIndex();
                IsLoaded = true;
            }
        }

        public void Clear()
        {
            records.Clear();
            _rebuildIndex.Clear();
        }

        protected virtual void RebuildIndex()
        {

        }

        protected void RebuildIndexByField<TIndex>(string fieldName)
        {
            //		object dic;
            Type recordType = typeof(TDataRecord);
            FieldInfo fieldInfo = recordType.GetField(fieldName);
            if (fieldInfo == null)
                throw new Exception("Field [" + fieldName + "] not found");
            IndexField<TIndex> indexField = new IndexField<TIndex>();
            _rebuildIndex[fieldName] = indexField;
            foreach (TDataRecord record in records)
            {
                var fieldValue = (TIndex)fieldInfo.GetValue(record);

                List<TDataRecord> indexedValue;
                if (!indexField.TryGetValue(fieldValue, out indexedValue))
                {
                    indexedValue = new List<TDataRecord>();
                    indexField[fieldValue] = indexedValue;
                }
                indexedValue.Add(record);

            }
        }



        public TDataRecord GetRecordByIndex<TIndex>(string fieldName, TIndex compareValue)
        {
            object dic = null;
            if (_rebuildIndex.TryGetValue(fieldName, out dic))
            {
                IndexField<TIndex> indexField = (IndexField<TIndex>)dic;
                List<TDataRecord> resultList = null;
                if (indexField.TryGetValue(compareValue, out resultList))
                    if (resultList.Count > 0) return resultList[0];

                return null;
            }
            return null;
        }

        public List<TDataRecord> GetRecordsByIndex<TIndex>(string fieldName, TIndex compareValue)
        {
            object dic = null;
            if (_rebuildIndex.TryGetValue(fieldName, out dic))
            {
                IndexField<TIndex> indexField = (IndexField<TIndex>)dic;
                List<TDataRecord> resultList = null;
                if (indexField.TryGetValue(compareValue, out resultList))
                    if (resultList.Count > 0) return resultList;

                return null;
            }
            return null;
        }

        object ConvertData(string value, Type t)
        {
            if (t.IsEnum)
            {
                Array arr = Enum.GetValues(t);
                if (string.IsNullOrEmpty(value))
                    return arr.GetValue(0);
                foreach (object item in arr)
                {
                    if (item.ToString().ToLower().Equals(value.Trim().ToLower()))
                        return item;
                }
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(t);
                if (typeCode == TypeCode.Int32)
                {
                    int result;
                    if (int.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.Int64)
                {
                    long result;
                    if (long.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.Int16)
                {
                    short result;
                    if (short.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.Single || typeCode == TypeCode.Decimal)
                {
                    float result;
                    if (float.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.String)
                {
                    //xu ly xuong dong
                    string[] regex = { @"\n" };
                    string[] temp2 = value.Split(regex, StringSplitOptions.None);
                    value = "";
                    for (int i = 0; i < temp2.Length; i++)
                    {
                        if (i == temp2.Length - 1)
                        {
                            value += temp2[i];
                            break;
                        }
                        value += temp2[i] + "\n";
                    }
                    return value;
                }
                else if (typeCode == TypeCode.Boolean)
                {
                    bool result;
                    if (bool.TryParse(value, out result))
                        return result;
                    if (value == "0")
                        return false;
                    else if (value == "1")
                        return true;
                }
                else if (typeCode == TypeCode.Double)
                {
                    double result;
                    if (double.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.DateTime)
                {
                    DateTime result;
                    if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss,fff",
                            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                        return result;
                    return null;
                }
            }
            return null;
        }

    }
}