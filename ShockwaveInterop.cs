using System.Runtime.InteropServices;

public static class ShockwaveInterop {
    [DllImport("ShockWaveIMU")]
    public static extern void StartShockwaveDevice();
    [DllImport("ShockWaveIMU")]
    public static extern void EnableBodyTracking();
    [DllImport("ShockWaveIMU")]
    public static extern void DisableBodyTracking();
   
    [DllImport("ShockWaveIMU")]
    public static extern void SetMaxCollisionIntensity(float maxIntensity);
    [DllImport("ShockWaveIMU")]
    public static extern void LEDUpdate(int[] index, float[] strengths, int len);
    [DllImport("ShockWaveIMU")]
    public static extern void HapticsUpdate(int[] index, float[] strengths, int len);
    [DllImport("ShockWaveIMU")]
    public static extern void HapticsPositionUpdate(int index, float strength, float pos);
    [DllImport("ShockWaveIMU")]
    public static extern void HapticsPulseWithPosition(int regionIndex, float value, float angYaw, float longitudinalPosition, float regionHeight, float milliseconds);
    [DllImport("ShockWaveIMU")]
    internal static extern void Quit();
    [DllImport("ShockWaveIMU")]
    internal static extern void SensorRotationInfo(int ind, ref float w, ref float x, ref float y, ref float z);
    [DllImport("ShockWaveIMU")]
    internal static extern void SensorRotationInfoRaw(int ind, ref float w, ref float x, ref float y, ref float z);
    [DllImport("ShockWaveIMU")]
    internal static extern void SendHeadsetPositionQuaternion(float[] headsetPos, float[] headsetQuaternion);
    [DllImport("ShockWaveIMU")]
    internal static extern void SendHeadsetPositionMatrix34(float[,] Matrix34);
    [DllImport("ShockWaveIMU")]
    internal static extern void StartPositionComputation(float customHeight);
    [DllImport("ShockWaveIMU")]
    internal static extern void StopPositionComputation();
    [DllImport("ShockWaveIMU")]
    internal static extern void GetPositionofTracker(int m_TrackerIndex, ref float posX, ref float posY, ref float posZ);
    [DllImport("ShockWaveIMU")]
    internal static extern bool isBoneTracked(int i);
    [DllImport("ShockWaveIMU")]
    internal static extern bool ConnectionStatus();
    [DllImport("ShockWaveIMU")]
    internal static extern void BodySizeData(ref float height, float[] boneLengths);
    [DllImport("ShockWaveIMU")]
    internal static extern  void HapticsPulse(int ind, float value, float milliseconds);
   
    [DllImport("ShockWaveIMU")]
    internal static extern bool suitConnected();



    [DllImport("ShockWaveIMU")]
    internal static extern void EnableAdditionalYaw(bool en);
   
   
}
