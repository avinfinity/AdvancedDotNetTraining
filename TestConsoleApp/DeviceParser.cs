using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class SingleDeviceData : DynamicObject
    {
        public Dictionary<string, string> _DeviceData = new Dictionary<string, string>();

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _DeviceData[binder.Name] = value.ToString();
            return true;
        }

        public void SetData(string columns, string data)
        {
            var columsNames = columns.Split(',');

            var datas = data.Split(',');

            for (int i = 0; i < columsNames.Length; i++)
            {
                _DeviceData[columsNames[i]] = datas[i];
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (!_DeviceData.ContainsKey(binder.Name))
                return false;

            result = _DeviceData[binder.Name];
            return true;
        }
    }

    class Devices : IEnumerable<DynamicObject>
    {

        private List<SingleDeviceData> _Data = new List<SingleDeviceData>();

        public Devices()
        {
            var lines = File.ReadAllLines("../../Devices.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                dynamic singleData = new SingleDeviceData();
                singleData.SetData(lines[0], lines[i]);
                _Data.Add(singleData);
            }
        }

        public IEnumerator<DynamicObject> GetEnumerator()
        {
            return _Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Data.GetEnumerator();
        }
    }
}
