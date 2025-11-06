using System;
using System.Linq;
using UnityEngine;

namespace DesignPatterns
{
    [Serializable]
    public class EnumData<E> : ISerializationCallbackReceiver where E : Enum
    {
        [SerializeField] private string _enumAssemblyQualifiedName;
        [SerializeField] private Data[] _data;
        [SerializeField] private MetaData[] _metaData;
        [SerializeField] private int _offset;

        public Data this[E key]
        {
            get => _data[Convert.ToInt32(key)];
            set
            {
                int index = Convert.ToInt32(key);
                _data[index] = value;
            }
        }

        public EnumData()
        {
            _enumAssemblyQualifiedName = typeof(E).FullName;
            InitializeData();
            UpdateMetaDataFromData();
        }

        public EnumData<E> GetCopy()
        {
            EnumData<E> copy = new EnumData<E>();
            for (int i = 0; i < copy._metaData.Length; i++) copy._metaData[i] = _metaData[i];
            copy.UpdateDataFromMetaData();
            return copy;
        }

        private void InitializeData()
        {
            int[] enumValues = (int[])Enum.GetValues(typeof(E));
            int max = enumValues.Max();
            int min = enumValues.Min();
            _offset = -min;
            _data = new Data[max - min];
            for (int i = 0; i < _data.Length; i++) _data[i] = 0;
        }

        public void OnBeforeSerialize()
        {
            UpdateMetaDataFromData();
        }

        private void UpdateMetaDataFromData()
        {
            if(_metaData == null || _metaData.Length != _data.Length) _metaData = new MetaData[_data.Length];
            for (int i = 0; i < _metaData.Length; i++)
            {
                _metaData[i] = new MetaData(_data[i].Value);
            }
        }

        private void UpdateDataFromMetaData()
        {
           if(_data == null || _data.Length != _metaData.Length) _data = new Data[_metaData.Length];
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = 0;
                _data[i].Value = JsonUtility.FromJson(_metaData[i].stringData, Type.GetType(_metaData[i].fullTypeName));
            }
        }

        public void OnAfterDeserialize()
        {
            if (_metaData == null || _metaData.Length != Enum.GetValues(typeof(E)).Length)
            {
                InitializeData();
                UpdateMetaDataFromData();
            }
            else UpdateDataFromMetaData();
        }

        private interface IData
        {
            public object Value { get; set; }
        }

        [Serializable]
        public class Data : IData
        {
            private object _value;
            public object Value
            {
                get => _value;
                set => _value = value;
            }
            private Data(object value) => _value = value;

            public static implicit operator Data(float value) => new Data(value);
            public static implicit operator float(Data data) => TypeConverter.ConvertObjectToType<float>(data._value);

            public static implicit operator Data(bool value) => new Data(value);
            public static implicit operator bool(Data data) => TypeConverter.ConvertObjectToType<bool>(data._value);

            public static implicit operator Data(string value) => new Data(value);
            public static implicit operator string(Data data) => TypeConverter.ConvertObjectToType<string>(data._value);

            public static implicit operator Data(int value) => new Data(value);
            public static implicit operator int(Data data) => TypeConverter.ConvertObjectToType<int>(data._value);

            public static implicit operator Data(Vector3 value) => new Data(value);
            public static implicit operator Vector3(Data data) => TypeConverter.ConvertObjectToType<Vector3>(data._value);

            public static implicit operator Data(Vector2 value) => new Data(value);
            public static implicit operator Vector2(Data data) => TypeConverter.ConvertObjectToType<Vector2>(data._value);
        }

        [Serializable]
        private struct MetaData
        {
            public string stringData;
            public string fullTypeName;
            public string shortTypeName;
            public MetaData(object data)
            {
                Type dataType = data.GetType();
                stringData = dataType.IsPrimitive ? data.ToString() : JsonUtility.ToJson(data);
                fullTypeName = dataType.AssemblyQualifiedName;
                shortTypeName = dataType.Name;
            }
        }
    }

    
}