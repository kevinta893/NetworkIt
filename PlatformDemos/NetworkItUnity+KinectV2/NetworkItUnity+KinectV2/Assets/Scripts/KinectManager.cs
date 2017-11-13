using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Windows.Kinect;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Use this script for programming advanced sensor based computer visioning.
/// </summary>
public class KinectManager : MonoBehaviour {

    //Data
    public int ColorWidth { get; private set; }
    public int ColorHeight { get; private set; }

    public int DepthWidth { get; private set; }
    public int DepthHeight { get; private set; }

    public int IRWidth { get; private set; }
    public int IRHeight { get; private set; }

    //Kinect sensor
    private KinectSensor _Sensor;
    private MultiSourceFrameReader _Reader;

    //color data
    private Texture2D _ColorTexture;
    private byte[] _ColorRawData;

    //depth data
    private ushort[] _DepthData;

    //ir Data
    private ushort[] _IRData;
    private byte[] _IRRawData;
    private Texture2D _IRTexture;

    //body data
    private Body[] _BodyData = null;
    
    // Use this for initialization
    void Start () {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Reader = _Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);

            //color
            FrameDescription colorFrameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = colorFrameDesc.Width;
            ColorHeight = colorFrameDesc.Height;
            _ColorTexture = new Texture2D(colorFrameDesc.Width, colorFrameDesc.Height, TextureFormat.RGBA32, false);
            _ColorRawData = new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];


            //depth
            FrameDescription depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
            DepthWidth = depthFrameDesc.Width;
            DepthHeight = depthFrameDesc.Height;

            _DepthData = new ushort[depthFrameDesc.LengthInPixels];


            //ir
            FrameDescription irFrameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            IRWidth = irFrameDesc.Width;
            IRHeight = irFrameDesc.Height;

            _IRData = new ushort[irFrameDesc.LengthInPixels];
            _IRRawData = new byte[irFrameDesc.LengthInPixels * 4];
            _IRTexture = new Texture2D(irFrameDesc.Width, irFrameDesc.Height, TextureFormat.RGBA32, false);


            //body
            BodyFrameReader bodyDesc = _Sensor.BodyFrameSource.OpenReader();

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }

        }
        else
        {
            Debug.LogError("Error! Kinect Sensor could not be started.");
            OnApplicationQuit();        //clear and close the kinect stream if possible.
        }
    }


    void Update () {
        UpdateKinect();                         //leave here, updates kinect sensor data for Unity

        //TODO, your Computer vision code here

        DemoIRBlobTrack();
    }

    private void DemoIRBlobTrack()
    {
        //get image and convert to threshold image
        Mat irImage = new Mat(IRWidth, IRHeight, MatType.CV_8UC4, _IRRawData);
        Mat ir8Bit = new Mat();
        Cv2.CvtColor(irImage, ir8Bit, ColorConversionCodes.RGBA2GRAY);
        Cv2.Threshold(ir8Bit, ir8Bit, thresh: 100, maxval: 255, type: ThresholdTypes.Binary);


        SimpleBlobDetector.Params detectorParams = new SimpleBlobDetector.Params
        {
            //MinDistBetweenBlobs = 10, // 10 pixels between blobs
            //MinRepeatability = 1,

            //MinThreshold = 100,
            //MaxThreshold = 255,
            //ThresholdStep = 5,

            FilterByArea = false,
            //FilterByArea = true,
            //MinArea = 0.001f, // 10 pixels squared
            //MaxArea = 500,

            FilterByCircularity = false,
            //FilterByCircularity = true,
            //MinCircularity = 0.001f,

            FilterByConvexity = false,
            //FilterByConvexity = true,
            //MinConvexity = 0.001f,
            //MaxConvexity = 10,

            FilterByInertia = false,
            //FilterByInertia = true,
            //MinInertiaRatio = 0.001f,

            FilterByColor = false
            //FilterByColor = true,
            //BlobColor = 255 // to extract light blobs
        };

        SimpleBlobDetector simpleBlobDetector = SimpleBlobDetector.Create(detectorParams);
        KeyPoint[] blobs = simpleBlobDetector.Detect(ir8Bit);
        
        Debug.Log("blobCount" + blobs.Length);

        Mat irImageKeyPoints = new Mat();
        Cv2.DrawKeypoints(ir8Bit, blobs, irImageKeyPoints, color: Scalar.FromRgb(255, 0, 0),
                    flags: DrawMatchesFlags.DrawRichKeypoints);

        Mat irImageOut = new Mat(IRWidth, IRHeight, MatType.CV_8UC4);
        Cv2.CvtColor(irImageKeyPoints, irImageOut, ColorConversionCodes.RGB2RGBA);

        simpleBlobDetector.Dispose();
        _IRTexture.LoadRawTextureData(ConvertMatToBytes(irImageOut));
        _IRTexture.Apply();
    }


    void UpdateKinect()
    {
        if (_Reader != null)
        {
            MultiSourceFrame frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {

                //color processing with depth
                ColorFrame colorFrame = frame.ColorFrameReference.AcquireFrame();
                if (colorFrame != null)
                {
                    DepthFrame depthFrame = frame.DepthFrameReference.AcquireFrame();
                    if (depthFrame != null)
                    {
                        colorFrame.CopyConvertedFrameDataToArray(_ColorRawData, ColorImageFormat.Rgba);
                        _ColorTexture.LoadRawTextureData(_ColorRawData);
                        _ColorTexture.Apply();

                        depthFrame.CopyFrameDataToArray(_DepthData);

                        depthFrame.Dispose();
                        depthFrame = null;
                    }

                    colorFrame.Dispose();
                    colorFrame = null;

                }

                //ir processing
                InfraredFrame irFrame = frame.InfraredFrameReference.AcquireFrame();
                if (irFrame != null)
                {
                    irFrame.CopyFrameDataToArray(_IRData);

                    int index = 0;
                    foreach (ushort ir in _IRData)
                    {
                        byte intensity = (byte)(ir >> 8);
                        _IRRawData[index++] = intensity;
                        _IRRawData[index++] = intensity;
                        _IRRawData[index++] = intensity;
                        _IRRawData[index++] = 255; // Alpha
                    }

                    //load raw data
                    _IRTexture.LoadRawTextureData(_IRRawData);
                    _IRTexture.Apply();

                    irFrame.Dispose();
                    irFrame = null;
                }


                //body processing
                BodyFrame bodyFrame = frame.BodyFrameReference.AcquireFrame();
                if (bodyFrame != null)
                {
                    if (_BodyData == null)
                    {
                        _BodyData = new Body[_Sensor.BodyFrameSource.BodyCount];
                    }
                    bodyFrame.GetAndRefreshBodyData(_BodyData);

                    bodyFrame.Dispose();
                    bodyFrame = null;

                }
                frame = null;
            }
        }
    }


    #region Debug Drawing functions
    //=================================================
    //Utility draw functions, dont forget to call Apply() on the texture when done.


    /// <summary>
    /// Draws a circle from center and radius. Expects points in the texture's coordinate system
    /// </summary>
    public static void DrawCircle(Texture2D tex, Vector2 pt, Color color, int radius)
    {
        int diameter = radius * 2;

        Vector2 center = new Vector2(diameter / 2, diameter / 2);
        int ptX = (int)pt.x;
        int ptY = (int)pt.y;


        for (int i = 0; i < diameter; i++)
        {
            for (int j = 0; j < diameter; j++)
            {
                Vector2 drawPt = new Vector2(i, j);

                if ((drawPt - center).sqrMagnitude <= (radius))
                {

                    tex.SetPixel(ptX + (i - radius), ptY + (j - radius), color);
                }
            }

        }


        tex.Apply();
    }

    /// <summary>
    /// Draws a line from start to end points. Expects points in the texture's coordinate system
    /// Source; http://wiki.unity3d.com/index.php?title=TextureDrawLine
    /// </summary>
    public static void DrawLine(Texture2D tex, Vector2 start, Vector2 end, Color color)
    {
        int x0 = (int) start.x;
        int y0 = (int) start.y;
        int x1 = (int) end.x;
        int y1 = (int) end.y;

        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, color);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, color);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, color);
            }
        }
        
    }


    /// <summary>
    /// Draws a rectangle. An option to have it filled.
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="topLeft"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="filled"></param>
    public static void DrawRectangle(Texture2D tex, Vector2 topLeft, int width, int height, Color color, bool filled)
    {
        //Easy, draw the 4 sides
        DrawLine(tex, topLeft, new Vector2(topLeft.x + width, topLeft.y), color);
        DrawLine(tex, topLeft, new Vector2(topLeft.x, topLeft.y + height), color);

        Vector2 bottomRight = new Vector2(topLeft.x + width, topLeft.y + height);
        DrawLine(tex, bottomRight, new Vector2(bottomRight.x - width, bottomRight.y), color);
        DrawLine(tex, bottomRight, new Vector2(bottomRight.x, bottomRight.y - height), color);


        //fill the rest of the rectangle
        if (filled)
        {
            int topLeftX = (int)topLeft.x;
            int topLeftY = (int)topLeft.y;

            //fill
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tex.SetPixel(topLeftX + i, topLeftY + j, color);
                }
            }
        }
    }
    #endregion

    #region Utility Functions

    public static byte[] ConvertMatToBytes(Mat img)
    {
        int renderSize = img.Width * img.Height * img.Channels();
        byte[] ret = new byte[renderSize];
        Marshal.Copy(img.Data, ret, 0, renderSize);
        return ret;
    }

    #endregion

    //=================================================
    //public accessor functions

    public Texture2D GetColorTexture()
    {
        return _ColorTexture;
    }

    //TextureFormat.RGBA32, 8-bit 4 channel
    public byte[] GetColorRawData()
    {
        return _ColorRawData;
    }

    public ushort[] GetDepthData()
    {
        return _DepthData;
    }


    public Texture2D GetInfraredTexture()
    {
        return _IRTexture;
    }

    //TextureFormat.BGRA32, 8-bit 4 channel
    public byte[] GetIRRawData()
    {
        return _IRRawData;
    }

    public Body[] GetBodyData()
    {
        return _BodyData;
    }



    //Turns off Kinect sensor when application closed
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
}
