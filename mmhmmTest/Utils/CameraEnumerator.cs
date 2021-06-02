using DirectShowLib;
using mmhmmTest.Model;
using System.Collections.Generic;
using System.Linq;

namespace mmhmmTest.Utils
{
    public static class CameraEnumerator
    {
        public static List<CameraModel> GetCameraList()
        {
            //var cams = new List<CameraModel>();
            var videoDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            int cameraIndex = 0;
            return videoDevices.Select(v => new CameraModel()
            {
                DeviceID = v.DevicePath,
                CameraName = v.Name,
                CameraID = cameraIndex++
            }).ToList();
        }
    }
}
