using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Windows.Kinect;



public class BlobTrackerDemo : MonoBehaviour {

    public KinectManager kinectManager;

    public Transform colorPlane;
    public Transform irPlane;

    public GameObject markerPrefab;


    // Use this for initialization
    void Start () {
		
	}

    // Must be called after KinectManager's update() function
    void LateUpdate()
    {
        //demo code, comment out or remove as necessary
        //Demo code and many more samples for OpenCVSharp can be found at: https://github.com/VahidN/OpenCVSharp-Samples
        DemoIRBlobTrack();
    }

    private Dictionary<int, GameObject> trackedBlobs = new Dictionary<int, GameObject>();

    private void DemoIRBlobTrack()
    {
        int IRWidth = kinectManager.IRWidth;
        int IRHeight = kinectManager.IRHeight;

        //get image and convert to threshold image
        Mat irImage = new Mat(IRHeight, IRWidth, MatType.CV_8UC4, kinectManager.GetIRRawData());              //rows=height, cols=width
        Mat ir8Bit = new Mat();
        Cv2.CvtColor(irImage, ir8Bit, ColorConversionCodes.RGBA2GRAY);
        Cv2.Threshold(ir8Bit, ir8Bit, thresh: 200, maxval: 255, type: ThresholdTypes.Binary);

        //Find blobs
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


        foreach (KeyPoint kp in blobs)
        {
            Debug.Log(kp.ClassId);

        }


        //convert back to unity texture, add nice debug drawings
        Mat irImageKeyPoints = new Mat();
        Cv2.DrawKeypoints(ir8Bit, blobs, irImageKeyPoints, color: Scalar.FromRgb(255, 0, 0),
                    flags: DrawMatchesFlags.DrawRichKeypoints);

        //Convert back to RGBA32
        Mat irImageOut = new Mat(IRWidth, IRHeight, MatType.CV_8UC4);
        Cv2.CvtColor(irImageKeyPoints, irImageOut, ColorConversionCodes.BGR2RGBA);      //OpenCV is weird and has it in BGR format

        //load onto texture
        byte[] rawTextureData = KinectCVUtilities.ConvertMatToBytes(irImageOut);
        kinectManager.GetIRTexture().LoadRawTextureData(rawTextureData);
        kinectManager.GetIRTexture().Apply();
    }


}
