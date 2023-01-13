using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System;
using System.Numerics;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
/*The singleton for managing all the suit utilizations*/

public class ShockwaveManager {
    public enum ERROR_ENUM {
        NO_ERROR = 0,
        NATIVE_INIT_ERROR = 1,
        STEAMVR_INIT_ERROR = 2,
        STEAMVR_32BIT_ERROR = 4,
        SUIT_MISSING_ERROR = 8,
        SUIT_MISSING_WAIT_TIMEOUT = 16
    }

    public static string GetErrorString(ERROR_ENUM err) {
        string strErr = "";
        
        if(err != 0) {
            if ((err & ERROR_ENUM.NATIVE_INIT_ERROR) > 0)
                strErr += "Native Initialization Error\n";
            if ((err & ERROR_ENUM.SUIT_MISSING_ERROR) > 0)
                strErr += "Shockwave is not connected to the Native Shockwave Driver\n";
            if ((err & ERROR_ENUM.STEAMVR_INIT_ERROR) > 0)
                strErr += "Shockwave can not connect to SteamVR\n";
            if ((err & ERROR_ENUM.SUIT_MISSING_ERROR) > 0)
                strErr += "Shockwave not found, check Native Shockwave Driver and SteamVR Driver\n";
            if ((err & ERROR_ENUM.SUIT_MISSING_WAIT_TIMEOUT) > 0)
                strErr += "Shockwave is not connected\n";
            if ((err & ERROR_ENUM.NATIVE_INIT_ERROR) > 0)
                strErr += "Shockwave is not connected to the Native Shockwave Driver\n";
            if ((err & ERROR_ENUM.STEAMVR_32BIT_ERROR) > 0)
                strErr += "Shockwave can only connect to SteamVR from a 64bit host application";
        }

        return strErr;
    }

    private bool useSteamVR = false;
    public bool isUsingSteamVR {  get { return useSteamVR; } }
    private static ShockwaveManager _instance;

    private ShockwaveManager() {
        _instance = this;
    }

    ~ShockwaveManager() {
        if (initialized)
        {
            DisconnectSuit();
        }
        if(_instance == this)
            _instance = null;
    }

  

    #region Tracking
    public bool enableBodyTracking = true;
    private bool PositionComputation = false;
    #endregion

    #region Haptics
    //[Tooltip("Maximum value of impulse to the body colliders that is expected in the scene.At this intensity the vibration in the suit is maximum.Higher values in the scene than the maximum intensity all clip to the max  ")]
    public float MaximumIntensity = 3;
    //[Tooltip("Intensity Dropoff for the haptics from the maximum intensity to 0.The default value of 4 implies a linear drop. A higher value means lower intensities of haptic exectutions in the scene will cause a large intensity of vibration ")]
    public int IntensityDropOff = 4;
    //[Tooltip("Intesity for continuous collisions inside body colliders.This is as a fraction of maximum intensity(0-1)")]
    //public float IntensityInsideCollider = 0.1f;
    #endregion

    public int error = 0;
    private bool initialized = false;

    public bool Ready => (_instance != null && _instance.initialized);

    public static ShockwaveManager Instance {
        get {
            return (_instance != null) ? 
                _instance : 
                (_instance = new ShockwaveManager());
        }
    }

   // Task suitMonitor = null;
    public void InitializeSuit() {
        try {
            ShockwaveInterop.StartShockwaveDevice();
            if (enableBodyTracking)
                ShockwaveInterop.EnableBodyTracking();
            initialized = true;
     
        } catch(Exception e) {
            error |= (int)ERROR_ENUM.NATIVE_INIT_ERROR;
            Console.WriteLine(e.Message);
            //Try SteamVR
         //   InitSteamVR();
        }
    }

/*
    uint steamvrInit;
    private void InitSteamVR() {
        try {
            steamvrInit = OpenVRInterop.GetInitToken();
            error |= ~(int)ERROR_ENUM.SUIT_MISSING_ERROR;
            //if () {
            //error |= (int)ERROR_ENUM.STEAMVR_INIT_ERROR;
            //return;
            //}
        } catch (Exception x) {
            //NOTE: Trying to load SteamVR from a 32bit executable is not possible and will throw an exception when trying to GetInitToken()
            error |= (int)ERROR_ENUM.STEAMVR_32BIT_ERROR;
            return;
        }

        FindSteamVRShockwaveTrackers();

        if (useSteamVR) {
            //Found SteamVR Hip Device
            initialized = true;
        } else {
            error |= (int)ERROR_ENUM.SUIT_MISSING_ERROR;
            _instance = null;
        }
    }
    */
    public enum HapticGroup {
        ALL = 0,
        HIP = 1,
        WAIST = 2,
        SPINE = 3,
        CHEST = 4,
        SHOULDERS = 5,
        LEFT_ARM = 6,
        LEFT_BICEP = 7,
        LEFT_FOREARM = 8,
        RIGHT_ARM = 9,
        RIGHT_BICEP = 10,
        RIGHT_FOREARM = 11,
        LEFT_LEG = 12,
        LEFT_THIGH = 13,
        LEFT_LOWER_LEG = 14,
        RIGHT_LEG = 15,
        RIGHT_THIGH = 16,
        RIGHT_LOWER_LEG = 17,

        FRONT = 18,
        HIP_FRONT = 19,
        LEFT_HIP_FRONT = 20,
        RIGHT_HIP_FRONT = 21,
        WAIST_FRONT = 22,
        LEFT_WAIST_FRONT = 23,
        RIGHT_WAIST_FRONT = 24,
        SPINE_FRONT = 25,
        LEFT_SPINE_FRONT = 26,
        RIGHT_SPINE_FRONT = 27,
        CHEST_FRONT = 28,
        LEFT_CHEST_FRONT = 29,
        RIGHT_CHEST_FRONT = 30,
        SHOULDERS_FRONT = 31,
        LEFT_SHOULDER_FRONT = 32,
        RIGHT_SHOULDER_FRONT = 33,
        LEFT_ARM_FRONT = 34,
        LEFT_BICEP_FRONT = 35,
        LEFT_FOREARM_FRONT = 36,
        RIGHT_ARM_FRONT = 37,
        RIGHT_BICEP_FRONT = 38,
        RIGHT_FOREARM_FRONT = 39,
        LEFT_LEG_FRONT = 40,
        LEFT_THIGH_FRONT = 41,
        LEFT_SHIN = 42,
        RIGHT_LEG_FRONT = 43,
        RIGHT_THIGH_FRONT = 44,
        RIGHT_SHIN = 45,

        BACK = 46,
        HIP_BACK = 47,
        LEFT_HIP_BACK = 48,
        RIGHT_HIP_BACK = 49,
        WAIST_BACK = 50,
        LEFT_WAIST_BACK = 51,
        RIGHT_WAIST_BACK = 52,
        SPINE_BACK = 53,
        LEFT_SPINE_BACK = 54,
        RIGHT_SPINE_BACK = 55,
        CHEST_BACK = 56,
        LEFT_CHEST_BACK = 57,
        RIGHT_CHEST_BACK = 58,
        SHOULDERS_BACK = 59,
        LEFT_SHOULDER_BACK = 60,
        RIGHT_SHOULDER_BACK = 61,
        LEFT_ARM_BACK = 62,
        LEFT_BICEP_BACK = 63,
        LEFT_FOREARM_BACK = 64,
        RIGHT_ARM_BACK = 65,
        RIGHT_BICEP_BACK = 66,
        RIGHT_FOREARM_BACK = 67,
        LEFT_LEG_BACK = 68,
        LEFT_THIGH_BACK = 69,
        LEFT_CALF = 70,
        RIGHT_LEG_BACK = 71,
        RIGHT_THIGH_BACK = 72,
        RIGHT_CALF = 73,
        LEFT_ELBOW = 74,
        RIGHT_ELBOW = 75,
        LEFT_KNEE = 76,
        RIGHT_KNEE = 77,
        LEFT_HIP = 78,
        RIGHT_HIP = 79,
        LEFT_WAIST = 80,
        RIGHT_WAIST = 81,
        LEFT_SPINE = 82,
        RIGHT_SPINE = 83,
        LEFT_CHEST = 84,
        RIGHT_CHEST = 85,
        LEFT_SHOULDER = 86,
        RIGHT_SHOULDER = 87,
        LEFT_SIDE = 88,
        RIGHT_SIDE = 89,
        PELVIS=90,
        PELVIS_SPINE=91,
        SPINE_CHEST=92,
        CHEST_SHOULDERS=93,
        SPINE_CHEST_SHOULDERS=94,
        TORSO=95,
        NUM_GROUPS = 96

    }
    public enum HapticRegion
    {TORSO=0,
        LEFTUPPERARM=1,
        LEFTLOWERARM=2,
        RIGHTUPPERARM=3,
        RIGHTLOWERARM=4,
        LEFTUPPERLEG=5,
        LEFTLOWERLEG=6,
        RIGHTUPPERLEG=7,
        RIGHTLOWERLEG=8
    }
    static readonly int[] allHaptics = {
      0, 1, 2, 3, 4, 5, 6, 7,
      8, 9, 10, 11, 12, 13, 14, 15,
      16, 17, 18, 19, 20, 21, 22, 23,
      24, 25, 26, 27, 28, 29, 30, 31,
      32, 33, 34, 35, 36, 37, 38, 39,
      40, 41, 42, 43, 44, 45, 46, 47,
      48, 49, 50, 51, 52, 53, 54, 55,
      56, 57, 58, 59, 60, 61, 62, 63,
      64, 65, 66, 67, 68, 69, 70, 71
    };

    //Front Regions
    static readonly int[] front = { 0, 1, 6, 7,
                    8, 9, 14, 15,
                    16, 17, 22, 23,
                    24, 25, 30, 31,
                    32, 33, 36, 37,
                    40, 42, 44, 46,
                    48, 50, 52, 54,
                    56, 58, 60, 62,
                    64, 66, 68, 70};

    static readonly int[] hips_front = { 0, 1, 6, 7 };
    static readonly int[] left_hip_front = { 0, 1 };
    static readonly int[] right_hip_front = { 6, 7 };

    static readonly int[] waist_front = { 8, 9, 14, 15 };
    static readonly int[] left_waist_front = { 8, 9 };
    static readonly int[] right_waist_front = { 14, 15 };

    static readonly int[] spine_front = { 16, 17, 22, 23 };
    static readonly int[] left_spine_front = { 22, 23 };
    static readonly int[] right_spine_front = { 16, 17 };

    static readonly int[] chest_front = { 24, 25, 30, 31 };
    static readonly int[] left_chest_front = { 24, 25 };
    static readonly int[] right_chest_front = { 30, 31 };

    static readonly int[] shoulders_front = { 32, 33, 36, 37 };
    static readonly int[] left_shoulder_front = { 32, 33 };
    static readonly int[] right_shoulder_front = { 36, 37 };

    static readonly int[] left_arm_front = { 40, 42, 44, 46 };
    static readonly int[] left_bicep_front = { 40, 42 };
    static readonly int[] left_forearm_front = { 44, 46 };

    static readonly int[] right_arm_front = { 48, 50, 52, 54 };
    static readonly int[] right_bicep_front = { 48, 50 };
    static readonly int[] right_forearm_front = { 52, 54};

    static readonly int[] left_leg_front = {56, 58, 60, 62};
    static readonly int[] left_thigh_front = { 56, 58};
    static readonly int[] left_shin = { 60, 62 };

    static readonly int[] right_leg_front = { 64, 66, 68, 70 };
    static readonly int[] right_thigh_front = { 64, 66 };
    static readonly int[] right_shin = { 68, 70 };

    //Back Regions
    static readonly int[] back = {
        2, 3, 4, 5,
        10, 11, 12, 13,
        18, 19, 20, 21,
        26, 27, 28, 29,
        34, 35, 38, 39,
        41, 43, 45, 47,
        49, 51, 53, 55,
        57, 59, 61, 63,
        65, 67, 69, 71
    };

    static readonly int[] hip_back = { 2, 3, 4, 5 };
    static readonly int[] left_hip_back = { 2, 3 };
    static readonly int[] right_hip_back = { 4, 5 };
    static readonly int[] waist_back = { 10, 11, 12, 13 };
    static readonly int[] left_waist_back = { 10, 11 };
    static readonly int[] right_waist_back = { 12, 13 };
    static readonly int[] spine_back = { 18, 19, 20, 21 };
    static readonly int[] left_spine_back = { 18, 19 };
    static readonly int[] right_spine_back = { 20, 21 };
    static readonly int[] chest_back = { 26, 27, 28, 29 };
    static readonly int[] left_chest_back = { 26, 27 };
    static readonly int[] right_chest_back = { 28, 29 };
    static readonly int[] shoulders_back = { 34, 35, 38, 39 };
    static readonly int[] left_shoulder_back = { 34, 35 };
    static readonly int[] right_shoulder_back = {38, 39};
    static readonly int[] left_arm_back = { 41, 43, 45, 47 };
    static readonly int[] left_bicep_back = { 41, 43 };
    static readonly int[] left_forearm_back = { 45, 47 };
    static readonly int[] right_arm_back = { 49, 51, 53, 55 };
    static readonly int[] right_bicep_back = { 49, 51 };
    static readonly int[] right_forearm_back = { 53, 55};
    static readonly int[] left_leg_back = { 57, 59, 61, 63 };
    static readonly int[] left_thigh_back = { 57, 59 };
    static readonly int[] left_calf = { 61, 63 };
    static readonly int[] right_leg_back = { 65, 67, 69, 71 };
    static readonly int[] right_thigh_back = { 65, 67 };
    static readonly int[] right_calf = { 69, 71 };

    //Regions
    static readonly int[] torso = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };

    static readonly int[] hip = { 0, 1, 2, 3, 4, 5, 6, 7 };
    static readonly int[] waist = { 8, 9, 10, 11, 12, 13, 14, 15 };
    static readonly int[] spine = { 16, 17, 18, 19, 20 ,21, 22, 23 };
    static readonly int[] chest = { 24, 25, 26, 27, 28, 29, 30, 31 };
    static readonly int[] pelvis = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    static readonly int[] pelvis_spine = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 ,16, 17, 18, 19, 20, 21, 22, 23 };
    static readonly int[] spine_chest = {16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
    static readonly int[] chest_shoulders = { 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
    static readonly int[] spine_chest_shoulders = { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
    static readonly int[] shoulders = { 32, 33, 34, 35, 36, 37, 38, 39 };
    static readonly int[] left_arm = { 40, 41, 42, 43, 44, 45, 46, 47};
    static readonly int[] left_bicep = { 40, 41, 42, 43 };
    static readonly int[] left_forearm = { 44, 45, 46, 47 };
    static readonly int[] right_arm = { 48, 49, 50, 51, 52, 53, 54, 55 };
    static readonly int[] right_bicep = { 48, 49, 50, 51 };
    static readonly int[] right_forearm = { 52, 53, 54, 55 };
    static readonly int[] left_leg = { 56, 57, 58, 59, 60, 61, 62, 63 };
    static readonly int[] left_thigh = { 56, 57, 58, 59 };
    static readonly int[] left_lower_leg = { 60, 61, 62, 63 };
    static readonly int[] right_leg = { 64, 65, 66, 67, 68, 69, 70, 71 };
    static readonly int[] right_thigh = { 64, 65, 66, 67 };
    static readonly int[] right_lower_leg = { 68, 69, 70, 71 };

    static readonly int[] left_elbow = { 43, 45 };
    static readonly int[] right_elbow = { 51, 53 };
    static readonly int[] left_knee = { 56, 60 };
    static readonly int[] right_knee = { 68, 64 };

    static readonly int[] left_hip = { 0, 1, 2, 3 };
    static readonly int[] right_hip = { 4, 5, 6, 7 };

    static readonly int[] left_waist = { 8, 9, 10, 11 };
    static readonly int[] right_waist = { 12, 13, 14, 15 };

    static readonly int[] left_spine = { 18, 19, 22, 23 };
    static readonly int[] right_spine = { 16, 17, 20, 21 };

    static readonly int[] left_chest = { 24, 25, 26, 27 };
    static readonly int[] right_chest = { 28, 29, 30, 31 };

    static readonly int[] left_shoulder = { 32, 33, 34, 35 };
    static readonly int[] right_shoulder = { 36, 37, 38, 39 };

    static readonly int[] left_side = {
        0, 1, 2, 3,
        8, 9, 10, 11,
        18, 19, 22, 23,
        24, 25, 26, 27,
        32, 33, 34, 35,
        40, 41, 42, 43, 44, 45, 46, 47,
        56, 57, 58, 59, 60, 61, 62, 63 
    };

    static readonly int[] right_side = {
        4, 5, 6, 7,
        12, 13, 14, 15,
        16, 17, 20, 21,
        28, 29, 30, 31,
        36, 37, 38, 39,
        48, 49, 50, 51, 52, 53, 54, 55,
        64, 65, 66, 67, 68, 69, 70, 71
    };

    public static int[] ShockGroup(HapticGroup group) {
        switch (group) {
            case HapticGroup.HIP:
                return hip;
            case HapticGroup.WAIST:
                return waist;
            case HapticGroup.SPINE:
                return spine;
            case HapticGroup.CHEST:
                return chest;
            case HapticGroup.SHOULDERS:
                return shoulders;
            case HapticGroup.LEFT_ARM:
                return left_arm;
            case HapticGroup.LEFT_BICEP:
                return left_bicep;
            case HapticGroup.LEFT_FOREARM:
                return left_forearm;
            case HapticGroup.RIGHT_ARM:
                return right_arm;
            case HapticGroup.RIGHT_BICEP:
                return right_bicep;
            case HapticGroup.RIGHT_FOREARM:
                return right_forearm;
            case HapticGroup.LEFT_LEG:
                return left_leg;
            case HapticGroup.LEFT_THIGH:
                return left_thigh;
            case HapticGroup.LEFT_LOWER_LEG:
                return left_lower_leg;
            case HapticGroup.RIGHT_LEG:
                return right_leg;
            case HapticGroup.RIGHT_THIGH:
                return right_thigh;
            case HapticGroup.RIGHT_LOWER_LEG:
                return right_lower_leg;
            case HapticGroup.FRONT:
                return front;
            case HapticGroup.HIP_FRONT:
                return hips_front;
            case HapticGroup.LEFT_HIP_FRONT:
                return left_hip_front;
            case HapticGroup.RIGHT_HIP_FRONT:
                return right_hip_front;
            case HapticGroup.WAIST_FRONT:
                return waist_front;
            case HapticGroup.LEFT_WAIST_FRONT:
                return left_waist_front;
            case HapticGroup.RIGHT_WAIST_FRONT:
                return right_waist_front;
            case HapticGroup.SPINE_FRONT:
                return spine_front;
            case HapticGroup.LEFT_SPINE_FRONT:
                return left_spine_front;
            case HapticGroup.RIGHT_SPINE_FRONT:
                return right_spine_front;
            case HapticGroup.CHEST_FRONT:
                return chest_front;
            case HapticGroup.LEFT_CHEST_FRONT:
                return left_chest_front;
            case HapticGroup.RIGHT_CHEST_FRONT:
                return right_chest_front;
            case HapticGroup.SHOULDERS_FRONT:
                return shoulders_front;
            case HapticGroup.LEFT_SHOULDER_FRONT:
                return left_shoulder_front;
            case HapticGroup.RIGHT_SHOULDER_FRONT:
                return right_shoulder_front;
            case HapticGroup.LEFT_ARM_FRONT:
                return left_arm_front;
            case HapticGroup.LEFT_BICEP_FRONT:
                return left_bicep_front;
            case HapticGroup.LEFT_FOREARM_FRONT:
                return left_forearm_front;
            case HapticGroup.RIGHT_ARM_FRONT:
                return right_arm_front;
            case HapticGroup.RIGHT_BICEP_FRONT:
                return right_bicep_front;
            case HapticGroup.RIGHT_FOREARM_FRONT:
                return right_forearm_front;
            case HapticGroup.LEFT_LEG_FRONT:
                return left_leg_front;
            case HapticGroup.LEFT_THIGH_FRONT:
                return left_thigh_front;
            case HapticGroup.LEFT_SHIN:
                return left_shin;
            case HapticGroup.RIGHT_LEG_FRONT:
                return right_leg_front;
            case HapticGroup.RIGHT_THIGH_FRONT:
                return right_thigh_front;
            case HapticGroup.RIGHT_SHIN:
                return right_shin;
            case HapticGroup.BACK:
                return back;
            case HapticGroup.HIP_BACK:
                return hip_back;
            case HapticGroup.LEFT_HIP_BACK:
                return left_hip_back;
            case HapticGroup.RIGHT_HIP_BACK:
                return right_hip_back;
            case HapticGroup.WAIST_BACK:
                return waist_back;
            case HapticGroup.LEFT_WAIST_BACK:
                return left_waist_back;
            case HapticGroup.RIGHT_WAIST_BACK:
                return right_waist_back;
            case HapticGroup.SPINE_BACK:
                return spine_back;
            case HapticGroup.LEFT_SPINE_BACK:
                return left_spine_back;
            case HapticGroup.RIGHT_SPINE_BACK:
                return right_spine_back;
            case HapticGroup.CHEST_BACK:
                return chest_back;
            case HapticGroup.LEFT_CHEST_BACK:
                return left_chest_back;
            case HapticGroup.RIGHT_CHEST_BACK:
                return right_chest_back;
            case HapticGroup.SHOULDERS_BACK:
                return shoulders_back;
            case HapticGroup.LEFT_SHOULDER_BACK:
                return left_shoulder_back;
            case HapticGroup.RIGHT_SHOULDER_BACK:
                return right_shoulder_back;
            case HapticGroup.LEFT_ARM_BACK:
                return left_arm_back;
            case HapticGroup.LEFT_BICEP_BACK:
                return left_bicep_back;
            case HapticGroup.LEFT_FOREARM_BACK:
                return left_forearm_back;
            case HapticGroup.RIGHT_ARM_BACK:
                return right_arm_back;
            case HapticGroup.RIGHT_BICEP_BACK:
                return right_bicep_back;
            case HapticGroup.RIGHT_FOREARM_BACK:
                return right_forearm_back;
            case HapticGroup.LEFT_LEG_BACK:
                return left_leg_back;
            case HapticGroup.LEFT_THIGH_BACK:
                return left_thigh_back;
            case HapticGroup.LEFT_CALF:
                return left_calf;
            case HapticGroup.RIGHT_LEG_BACK:
                return right_leg_back;
            case HapticGroup.RIGHT_THIGH_BACK:
                return right_thigh_back;
            case HapticGroup.RIGHT_CALF:
                return right_calf;
            case HapticGroup.LEFT_ELBOW:
                return left_elbow;
            case HapticGroup.RIGHT_ELBOW:
                return right_elbow;
            case HapticGroup.LEFT_KNEE:
                return left_knee;
            case HapticGroup.RIGHT_KNEE:
                return right_knee;
            case HapticGroup.LEFT_HIP:
                return left_hip;
            case HapticGroup.RIGHT_HIP:
                return right_hip;
            case HapticGroup.LEFT_WAIST:
                return left_waist;
            case HapticGroup.RIGHT_WAIST:
                return right_waist;
            case HapticGroup.LEFT_SPINE:
                return left_spine;
            case HapticGroup.RIGHT_SPINE:
                return right_spine;
            case HapticGroup.LEFT_CHEST:
                return left_chest;
            case HapticGroup.RIGHT_CHEST:
                return right_chest;
            case HapticGroup.LEFT_SHOULDER:
                return left_shoulder;
            case HapticGroup.RIGHT_SHOULDER:
                return right_shoulder;
            case HapticGroup.LEFT_SIDE:
                return left_side;
            case HapticGroup.RIGHT_SIDE:
                return right_side;
            case HapticGroup.PELVIS:
                return pelvis;
            case HapticGroup.PELVIS_SPINE:
                return pelvis_spine;
            case HapticGroup.SPINE_CHEST:
                return spine_chest;
            case HapticGroup.CHEST_SHOULDERS:
                return chest_shoulders;
            case HapticGroup.SPINE_CHEST_SHOULDERS:
                return spine_chest_shoulders;
            case HapticGroup.TORSO:
                return torso;
            default:
                return allHaptics;
        }
    }

    public bool suitConnected()
    {
        return ShockwaveInterop.suitConnected();
    }
    bool testing = false;
    
    private async Task HapticsTest(int msTestDuration, float strength) {
        if (!testing) {
            testing = true;
            
            int TestNum = 1;


            while (TestNum < 18) {
             
                    
                    TestNum++;
                   
                        SendHapticGroupTest((HapticGroup)TestNum, strength, msTestDuration);
                    

                await Task.Delay(200);
            }

            testing = false;
        }
    }

    private void SendHapticGroupTest(HapticGroup group, float strength, int msDuration) {
        if (testing) {
            int[] indices = ShockGroup(group);
            float[] strengths = new float[indices.Length];
            for (int h = 0; h < indices.Length; h++)
            { strengths[h] = strength;
              ShockwaveInterop.HapticsPulse(indices[h], strengths[h], msDuration);
            }
        }
    }
/*
    private async Task HapticsPulse(int[] indices, float[] strengths, int msDuration) {
        if (msDuration > 0) {
            System.DateTime end = System.DateTime.UtcNow.AddMilliseconds(msDuration);

            while (System.DateTime.UtcNow < end) {
                sendHapticsUpdate(indices, strengths, indices.Length);
                await Task.Delay(1);
            }
        } else sendHapticsUpdate(indices, strengths, indices.Length);
    }
*/
    public void SendHapticGroup(HapticGroup group, float strength, int msDuration) {
        if (!testing) {
            int[] indices = ShockGroup(group);
            float[] strengths = new float[indices.Length];
            for (int h = 0; h < indices.Length; h++)
            { strengths[h] = strength;
                ShockwaveInterop.HapticsPulse(indices[h], strengths[h], msDuration);
            }

           
        }
    }

    public void InitSequence() {
        if(!testing)
            Task.Run(() => HapticsTest(100, 1.0f));
    }
/*
    void FindSteamVRShockwaveTrackers() {
        StringBuilder str = new System.Text.StringBuilder((int)64);
        ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;

        try {
            for (hipIndex = 1; hipIndex < 32; hipIndex++) {
                OpenVR.System.GetStringTrackedDeviceProperty(hipIndex, ETrackedDeviceProperty.Prop_SerialNumber_String, str, OpenVR.k_unMaxPropertyStringSize, ref error);
                if (str.ToString() == "Shockwave_Hip") {
                    useSteamVR = true;
                    break;
                }
            }
        } catch(Exception e) {
            Console.WriteLine($"Index {hipIndex} out of bounds");
        }
    }
    */
    public void StartPositionComputation(float customHeight) {
        if (!PositionComputation) {
            PositionComputation = true;
            ShockwaveInterop.StartPositionComputation(customHeight);
        }
    }

    public void StopPositionComputation() {
        if (PositionComputation) {
            PositionComputation = false;
            ShockwaveInterop.StopPositionComputation();
        }
    }
    public void SendHeadsetPositionQuaternion(float[] headsetPos, float[] headsetQuaternion)
    {
        ShockwaveInterop.SendHeadsetPositionQuaternion(headsetPos, headsetQuaternion);
    }
    public void DisconnectSuit() {
        initialized = false;

        if (!useSteamVR) {
            StopPositionComputation();
            ShockwaveInterop.Quit();
        }
    }

    public float[] GetRotations(int index) {
        float[] r = {0, 0, 0, 1};
        ShockwaveInterop.SensorRotationInfo(index, ref r[0], ref r[1], ref r[2], ref r[3]);
        return r;

        //TODO: Add SteamVR Support
    }

    public float[] GetTrackerPosition(int index) {
        float[] r = { 0, 0, 0 };
        ShockwaveInterop.GetPositionofTracker(index, ref r[0], ref r[1], ref r[2]);
        return r;
        //TODO: Add SteamVR Support
    }

    public float GetBodyHeight() {
        float h = 1.7f;
        float[] f = new float[8];
        ShockwaveInterop.BodySizeData(ref h, f);
        return h;
        //TODO: Add SteamVR Support
    }

    public float[] GetBoneLengths() {
        float h = 1.7f;
        float[] f = new float[8];
        ShockwaveInterop.BodySizeData(ref h, f);
        return f;
        //TODO: Add SteamVR Support
    }

    public void EnableBodyTracking(bool enabled = true) {
        if (enabled && !enableBodyTracking) {
            ShockwaveInterop.EnableBodyTracking();
            enableBodyTracking = true;
        } else if (!enabled && enableBodyTracking) {
            ShockwaveInterop.DisableBodyTracking();
            enableBodyTracking = false;
        }
    }

    public bool isBoneTracked(int index) {
        //TODO: Add SteamVR Support
        return ShockwaveInterop.isBoneTracked(index);
    }

    public void sendHapticsUpdate(int[] hapticIndices, float[] hapticStrengths, int numberActuated) {
       
            ShockwaveInterop.HapticsUpdate(hapticIndices, hapticStrengths, numberActuated);
    }

    public void sendHapticsPulse(int hapticIndices, float hapticStrengths, float milliseconds)
    {
        

            ShockwaveInterop.HapticsPulse(hapticIndices, hapticStrengths, milliseconds);
        
    }
    public void sendHapticsPulse(int[] hapticIndices, float[] hapticStrengths, float milliseconds)
    {
        for (int h = 0; h < Math.Min(hapticIndices.Length,hapticStrengths.Length); h++)
        {
            
            ShockwaveInterop.HapticsPulse(hapticIndices[h], hapticStrengths[h], milliseconds);
        }
    }
    public void sendHapticsPulsewithPositionInfo(HapticRegion region, float value, float angYaw, float longitudinalPosition, float regionHeight, float milliseconds)
    {
        ShockwaveInterop.HapticsPulseWithPosition((int)region,value, angYaw, longitudinalPosition, regionHeight, milliseconds);
    }
    uint hipIndex = uint.MaxValue;
    /*
    public void sendSteamVRHapticsUpdate(int[] hapticIndices, float[] hapticStrengths, int numberActuated) {
        if (hipIndex == 0)
            return;
        if (numberActuated > 9)
            numberActuated = 9;
        for (uint i = 0; i < numberActuated; i++) { OpenVR.System.TriggerHapticPulse(hipIndex + i + 1, 0, (ushort)((hapticIndices[i] * 50) + (hapticStrengths[i] * 49))); }
    }
    */

    public void sendLEDUpdate(int[] ledIndices, float[] ledStrengths, int numberActuated) {
        ShockwaveInterop.LEDUpdate(ledIndices, ledStrengths, numberActuated);
    }

    public void sendHapticsWithPosition(int hapticIndex, float hapticStrength, float relativeDistance) {
            ShockwaveInterop.HapticsPositionUpdate(hapticIndex, hapticStrength, relativeDistance);
    }

    ushort hap = 0;
    /*
    public void sendSteamVRHapticsWithPosition(int hapticIndex, float hapticStrength, float position) {
        if (hipIndex == 0 || hap >= 8)
            return;

        OpenVR.System.TriggerHapticPulse(hipIndex + hap++, 0, (ushort)((hapticIndex * 50) + (position * 49)));
        OpenVR.System.TriggerHapticPulse(hipIndex + hap++, 0, (ushort)((hapticIndex * 50) + (hapticStrength * 49)));

        if (hap > 127)
            hap -= 127;
    }*/

    public void sendLEDUpdate(float[] color, bool leftSide) {
        int[] ledIndices = new int[3];
        float[] ledStrengths = new float[3] { color[1], color[0], color[2] };
        for (int i = 0; i < 3; i++)
            ledIndices[i] = leftSide ? i : i + 3;
        ShockwaveInterop.LEDUpdate(ledIndices, ledStrengths, 3);
    }


    public void SetMaximumIntensity(float maximumIntensity) {
        MaximumIntensity = maximumIntensity;
        ShockwaveInterop.SetMaxCollisionIntensity(maximumIntensity);
    }
}
