using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Windows.Kinect;



public class BlobTrackerDemo : MonoBehaviour {

    public KinectManager kinectManager;

    public Transform irPlane;

    public GameObject markerPrefab;


    private Dictionary<int, GameObject> trackedBlobs = new Dictionary<int, GameObject>();


    // Use this for initialization
    void Start () {
        theTrack = Instantiate(markerPrefab);
	}

    // Must be called after KinectManager's update() function
    void LateUpdate()
    {
        //demo code, comment out or remove as necessary
        //Demo code and many more samples for OpenCVSharp can be found at: https://github.com/VahidN/OpenCVSharp-Samples
        DemoIRBlobTrack();
    }
    GameObject theTrack;

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
            
            Vector2 blobPt = new Vector2(kp.Pt.X, kp.Pt.Y);
            Debug.Log(blobPt);
            theTrack.transform.position = TransformIRToUnity(blobPt);
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

    private Vector3 TransformIRToUnity(Vector2 irPoint)
    {
        Vector2 worldCoord = new Vector2();
        worldCoord.x = irPoint.x;
        worldCoord.y = irPoint.y;


        Vector2 planePos = new Vector2(irPlane.transform.position.x, irPlane.transform.position.y);

        float irWidth = kinectManager.IRWidth;
        float irHeight = kinectManager.IRHeight;

        float planeWidth = irPlane.localScale.x;
        float planeHeight = irPlane.localScale.y;



        //scale the local pixel system to the unity world system.
        Vector2 scaleTransform = new Vector2(planeWidth / irWidth, planeHeight / irHeight);
        worldCoord = Vector2.Scale(worldCoord, scaleTransform);

        //invert the y since y0 starts from bottom up
        worldCoord.y = planeHeight - worldCoord.y;

        //transform to real world, the pixel point is in the unity world coord system
        worldCoord += planePos;

        //convert to plane's coordinate system, plane's have their origins (world position) start at the center of the object
        worldCoord.x -= planeWidth / 2;
        worldCoord.y -= planeHeight / 2;


        return new Vector3(worldCoord.x, worldCoord.y, irPlane.transform.position.z);
    }
}
