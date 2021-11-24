using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IPC
{
    public static class Ipc
    {
        
        [DllImport("sem", EntryPoint = "ReadInt", CharSet = CharSet.Ansi)]
        private static extern int ReadInt(int mmap, int size);
        
        [DllImport("sem", EntryPoint = "WriteInt", CharSet = CharSet.Ansi)]
        private static extern int WriteInt(int val, int mmap);
        
        [DllImport("sem", EntryPoint = "semaphore_open", CharSet = CharSet.Ansi)]
        private static extern int semaphore_open(string semname, int oflag, int val);
        
        [DllImport("sem", EntryPoint = "getO_Creat", CharSet = CharSet.Ansi)]
        private static extern int getO_Creat();
        
        [DllImport("sem", EntryPoint = "wait", CharSet = CharSet.Ansi)]
        private static extern void wait(int ind);
        
        [DllImport("sem", EntryPoint = "post", CharSet = CharSet.Ansi)]
        private static extern void post(int ind);
        
        [DllImport("sem", EntryPoint = "shared_mem", CharSet = CharSet.Ansi)]
        private static extern int shared_mem(string name, int size);

        [DllImport("sem", EntryPoint = "writeMMF", CharSet = CharSet.Ansi)]
        private static extern void writeMMF(string msg, int mmap);
        
        [DllImport("sem", EntryPoint = "readMMF")]
        private static extern IntPtr readMMF(int mmap);

        [DllImport("sem", EntryPoint = "reset", CharSet = CharSet.Ansi)]
        private static extern void reset();
        
        private static bool _isReset;

        private static readonly int PinetImgLock;
        private static readonly int PinetOutputLock;
        private static readonly int PinetOutputReadyLock;
        
        private static readonly int PinetImgMmf;
        private static readonly int PinetOutputMmf;
        private static readonly int PinetOutputReadyMmf;
        
        private static readonly int LidarImgLock;
        private static readonly int LidarOutputLock;
        private static readonly int LidarOutputReadyLock;
        
        private static readonly int LidarImgMmf;
        private static readonly int LidarOutputMmf;
        private static readonly int LidarOutputReadyMmf;
        
        private static readonly int YoloImgLock;
        private static readonly int YoloOutputLock;
        private static readonly int YoloOutputReadyLock;
        
        private static readonly int YoloImgMmf;
        private static readonly int YoloOutputMmf;
        private static readonly int YoloOutputReadyMmf;
        
        private static readonly int DeterministicImgLock;
        private static readonly int DeterministicOutputLock;
        private static readonly int DeterministicOutputReadyLock;
        
        private static readonly int DeterministicImageMmf;
        private static readonly int DeterministicOutputMmf;
        private static readonly int DeterministicOutputReadyMmf;

        private static readonly int MappingImgLock;
        private static readonly int MappingReadyToReceiveLock;
        private static readonly int MapperOutputLock;
        
        private static readonly int MappingLeftImgMmf;
        private static readonly int MappingRightImgMmf;
        private static readonly int MappingTimeMmf;
        private static readonly int MappingReadyToReceiveMmf;
        private static readonly int MapperOutputMmf;

        private static readonly int MapUnityCoordLock;
        private static readonly int MapCoordOutputLock;
        private static readonly int MapOutputReadyLock;
        
        private static readonly int MapUnityCoordMmf;
        private static readonly int MapCoordOutputMmf;
        private static readonly int MapOutputReadyMmf;
        
        private static readonly  int DepthImageMmf; 
        private static readonly  int PCSegmentationOutputMmf;
        private static readonly  int PCSegmentationOutputReadyMmf;
        private static readonly  int PCSegmentationOutputLock;
        private static readonly  int DepthImageLock;
        private static readonly  int PCSegmentationOutputReadyLock;
        
        static Ipc()
        {
            PinetImgMmf                   = shared_mem("pinet_image_mmf", 1000000);
            PinetOutputMmf                = shared_mem("pinet_output_mmf", 32768);
            PinetOutputReadyMmf           = shared_mem("pinet_ready_mmf", 4);
            
            PinetImgLock                  = semaphore_open("pinet_image_lock", getO_Creat(), 1);
            PinetOutputLock               = semaphore_open("pinet_output_lock", getO_Creat(), 1);
            PinetOutputReadyLock          = semaphore_open("pinet_ready_lock", getO_Creat(), 1);
            
            LidarImgMmf                   = shared_mem("lidar_image_mmf", 1000000);
            LidarOutputMmf                = shared_mem("lidar_output_mmf", 65536);
            LidarOutputReadyMmf           = shared_mem("lidar_ready_mmf", 4);
            
            LidarImgLock                  = semaphore_open("lidar_image_lock", getO_Creat(), 1);
            LidarOutputLock               = semaphore_open("lidar_output_lock", getO_Creat(), 1);
            LidarOutputReadyLock          = semaphore_open("lidar_ready_lock", getO_Creat(), 1);
            
            YoloImgMmf                    = shared_mem("yolo_image_mmf", 1000000);
            YoloOutputMmf                 = shared_mem("yolo_output_mmf", 256);
            YoloOutputReadyMmf            = shared_mem("yolo_ready_mmf", 4);
            
            YoloImgLock                   = semaphore_open("yolo_image_lock", getO_Creat(), 1);
            YoloOutputLock                = semaphore_open("yolo_output_lock", getO_Creat(), 1);
            YoloOutputReadyLock           = semaphore_open("yolo_ready_lock", getO_Creat(), 1);
            
            DeterministicImageMmf         = shared_mem("deterministic_image_mmf", 1000000);
            DeterministicOutputMmf        = shared_mem("deterministic_output_mmf", 32768);
            DeterministicOutputReadyMmf   = shared_mem("deterministic_ready_mmf", 4);
            
            DeterministicImgLock          = semaphore_open("deterministic_image_lock", getO_Creat(), 1);
            DeterministicOutputLock       = semaphore_open("deterministic_output_lock", getO_Creat(), 1);
            DeterministicOutputReadyLock  = semaphore_open("deterministic_ready_lock", getO_Creat(), 1);
            
            MappingLeftImgMmf             = shared_mem("mapping_left_image_mmf", 100000000);
            MappingRightImgMmf            = shared_mem("mapping_right_image_mmf", 100000000);
            MappingTimeMmf                = shared_mem("mapping_time_mmf", 4);
            MappingReadyToReceiveMmf      = shared_mem("mapping_ready_to_receive", 4);
            MapperOutputMmf               = shared_mem("mapping_output_mmf", 32768);

            MapperOutputLock              = semaphore_open("mapping_output_lock", getO_Creat(), 1);
            MappingImgLock                = semaphore_open("mapping_image_lock", getO_Creat(), 1);
            MappingReadyToReceiveLock     = semaphore_open("mapping_ready_to_receive_lock", getO_Creat(), 1);
            
            MapUnityCoordMmf              = shared_mem("map_unity_coord_mmf", 100);
            MapCoordOutputMmf             = shared_mem("map_coord_output_mmf", 300);
            MapOutputReadyMmf             = shared_mem("map_output_ready_mmf", 4);
            
            MapUnityCoordLock             = semaphore_open("map_unity_coord_lock", getO_Creat(), 1);
            MapCoordOutputLock            = semaphore_open("map_coord_output_lock", getO_Creat(), 1);
            MapOutputReadyLock            = semaphore_open("map_output_ready_lock", getO_Creat(), 1);
            
            DepthImageMmf                 = shared_mem("depth_image_mmf", 102400);
            PCSegmentationOutputMmf       = shared_mem("pc_segmentation_output_mmf", 10240);
            PCSegmentationOutputReadyMmf  = shared_mem("pc_segmentation_output_ready_mmf", 4);
            
            PCSegmentationOutputLock      = semaphore_open("pc_segmentation_output_lock", getO_Creat(), 1);
            DepthImageLock                = semaphore_open("depth_image_lock", getO_Creat(), 1);
            PCSegmentationOutputReadyLock = semaphore_open("pc_segmentation_output_ready_lock", getO_Creat(), 1);
        }

        public static void Reset()
        {
            if (!_isReset)
            {
                reset();
                _isReset = true;
                Debug.Log("All locks and semaphores have been reset.");
            }
        }
        
        // //////////////////////////////////////////////// PINET IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static bool IsPinetOutputReady()
        {
            return ReadInt(PinetOutputReadyMmf, 4) == 1;
        }
        
        public static string ReadPinetOutput()
        {
            wait(PinetOutputLock);
            var output = readMMF(PinetOutputMmf);
            post(PinetOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WritePinetImg(string img)
        {
            wait(PinetImgLock);
            writeMMF(img, PinetImgMmf);
            post(PinetImgLock);
        }

        public static void UnsetPinetOutputReady()
        {
            wait(PinetOutputReadyLock);
            WriteInt(0, PinetOutputReadyMmf);
            post(PinetOutputReadyLock);
        }
        
        // //////////////////////////////////////////////// LIDAR IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static bool IsLidarOutputReady()
        {
            return ReadInt(LidarOutputReadyMmf, 4) == 1;
        }
        public static string ReadLidarOutput()
        {
            wait(LidarOutputLock);
            var output = readMMF(LidarOutputMmf);
            post(LidarOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WriteLidarImg(string img)
        {
            wait(LidarImgLock);
            writeMMF(img, LidarImgMmf);
            post(LidarImgLock);
        }
        public static void UnsetLidarOutputReady()
        {
            wait(LidarOutputReadyLock);
            WriteInt(0, LidarOutputReadyMmf);
            post(LidarOutputReadyLock);
        }
        
        // //////////////////////////////////////////////// YOLO IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static bool IsYoloOutputReady()
        {
            return ReadInt(YoloOutputReadyMmf, 4) == 1;
        }
        public static string ReadYoloOutput()
        {
            wait(YoloOutputLock);
            var output = readMMF(YoloOutputMmf);
            post(YoloOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WriteYoloImg(string img)
        {
            wait(YoloImgLock);
            writeMMF(img, YoloImgMmf);
            post(YoloImgLock);
        }
        public static void UnsetYoloOutputReady()
        {
            wait(YoloOutputReadyLock);
            WriteInt(0, YoloOutputReadyMmf);
            post(YoloOutputReadyLock);
        }
        
        // ////////////////////////////////////////// DETERMINISTIC-LD IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static bool IsDeterministicLdOutputReady()
        {
            return ReadInt(DeterministicOutputReadyMmf, 4) == 1;
        }
        
        public static string ReadDeterministicLdOutput()
        {
            wait(DeterministicOutputLock);
            var output = readMMF(DeterministicOutputMmf);
            post(DeterministicOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WriteDeterministicLdImg(string img)
        {
            wait(DeterministicImgLock);
            writeMMF(img, DeterministicImageMmf);
            post(DeterministicImgLock);
        }

        public static void UnsetDeterministicLdOutputReady()
        {
            wait(DeterministicOutputReadyLock);
            WriteInt(0, DeterministicOutputReadyMmf);
            post(DeterministicOutputReadyLock);
        }
        
        // ////////////////////////////////////////// ORB-SLAM Mapping IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static void WriteMappingImgAndTime(string leftImg, string rightImg, string time)
        {
            wait(MappingImgLock);
            writeMMF(leftImg, MappingLeftImgMmf);
            writeMMF(rightImg, MappingRightImgMmf);
            writeMMF(time, MappingTimeMmf);
            post(MappingImgLock);
        }

        public static bool IsMapperReadyToReceive()
        {
            return ReadInt(MappingReadyToReceiveMmf, 4) == 1;
        }
        
        public static void UnsetMapperReadyToReceive()
        {
            wait(MappingReadyToReceiveLock);
            WriteInt(0, MappingReadyToReceiveMmf);
            post(MappingReadyToReceiveLock);
        }
        
        public static string ReadMapperOutput()
        {
            wait(MapperOutputLock);
            var output = readMMF(MapperOutputMmf);
            post(MapperOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        
        // /////////////////////////////////////////// MAP IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static bool IsMapProviderReady()
        {
            return ReadInt(MapOutputReadyMmf, 4) == 1;
        }

        public static string ReadMapOutput()
        {
            wait(MapCoordOutputLock);
            var output = readMMF(MapCoordOutputMmf);
            post(MapCoordOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }

        public static void WriteUnityCoordinate(string unityCoords)
        {
            wait(MapUnityCoordLock);
            writeMMF(unityCoords, MapUnityCoordMmf);
            post(MapUnityCoordLock);
        }

        public static void UnsetMapOutputReady()
        {
            wait(MapOutputReadyLock);
            WriteInt(0, MapOutputReadyMmf);
            post(MapOutputReadyLock);
        }

        // /////////////////////////////////////// PC Segmentation IPC \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        public static bool IsPCSegmentationOutputReady()
        {
            return ReadInt(PCSegmentationOutputReadyMmf, 4) == 1;
        }
        public static string ReadPCSegmentationOutput()
        {
            wait(PCSegmentationOutputLock);
            var output = readMMF(PCSegmentationOutputMmf);
            post(PCSegmentationOutputLock);
            var strResult = Marshal.PtrToStringAnsi(output);
            return strResult;
        }
        public static void WriteDepth(string depthImage)
        {
            wait(DepthImageLock);
            writeMMF(depthImage, DepthImageMmf);
            post(DepthImageLock);
        }
        public static void UnsetPCSegmentationOutputReady()
        {
            wait(PCSegmentationOutputReadyLock);
            WriteInt(0, PCSegmentationOutputReadyMmf);
            post(PCSegmentationOutputReadyLock);
        } 
        
    }
}
    