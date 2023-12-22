using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class ArtificialLoadPort : ILoadPort
    {
        private bool isMapping = true;
        private bool?[] slot;

        public ArtificialLoadPort()
        {
            slot = new bool?[] { true };

        }

        public bool IsMapping => isMapping;

        public bool?[] Slot => slot;

        public string ErrorStatus => throw new NotImplementedException();

        public string DeviceStatus => throw new NotImplementedException();

        public string ErrorCode => throw new NotImplementedException();

        public bool IsCassettePutOK => throw new NotImplementedException();

        public bool IsClamp => throw new NotImplementedException();

        public bool IsSwitchDoor => throw new NotImplementedException();

        public bool IsVaccum => throw new NotImplementedException();

        public bool IsDoorOpen => throw new NotImplementedException();

        public bool IsSensorCheckDoorOpen => throw new NotImplementedException();

        public bool IsDock => throw new NotImplementedException();

        public int WaferThickness { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CassettePitch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int StarOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WaferPitchTolerance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WaferPositionTolerance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void GetParam()
        {
            throw new NotImplementedException();
        }

        public void GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Home()
        {

        }

        public void Initial()
        {

        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void AlarmReset()
        {
            throw new NotImplementedException();
        }

        public void SetParam()
        {
            throw new NotImplementedException();
        }

        public void SlotMapping()
        {

        }
    }
}
