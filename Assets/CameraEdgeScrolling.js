var mDelta = 10; // Pixels. The width border at the edge in which the movement work
var mSpeed = 10.0; // Scale. Speed of the movement
 
var xSensitivity = 5.0;
var ySensitivity = 5.0;
var xAngle = 0.0;
var yAngle = 0.0;
 
 
private var mForwardDirection : Vector3; // What direction does our camera start looking at
private var mRightDirection : Vector3; // The inital "right" of the camera
private var mUpDirection : Vector3; // The inital "up" of the camera
 
function Start()
{
   mForwardDirection = transform.forward;
   mRightDirection = transform.right;
   mUpDirection = transform.up;
}
 
function LateUpdate ()
{
 
   // Omars "change position part"
   // Check if on the right edge
   if ( Input.mousePosition.x >= Screen.width - mDelta && transform.position.x < 20 )
   {
      // Move the camera
      transform.position += mRightDirection * Time.deltaTime * mSpeed;
   }
 
 
   if ( Input.mousePosition.x <= 0 + mDelta && transform.position.x > 0 )
   {
      // Move the camera
      transform.position -= mRightDirection * Time.deltaTime * mSpeed;
   }
 
 
   if ( Input.mousePosition.y >= Screen.width - mDelta )
   {
      // Move the camera
      transform.position += mUpDirection * Time.deltaTime * mSpeed;
   }
   //Debug.Log(transform.position);
}