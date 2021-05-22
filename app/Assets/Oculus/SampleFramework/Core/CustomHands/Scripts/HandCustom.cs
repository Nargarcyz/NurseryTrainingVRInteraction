/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided �AS IS� WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using OVRTouchSample;
using VRTK;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace OVRTouchSample
{
    // Animated hand visuals for a user of a Touch controller.
    [RequireComponent(typeof(OVRGrabber))]
    public class HandCustom : MonoBehaviour
    {
        public const string ANIM_LAYER_NAME_POINT = "Point Layer";
        public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";
        public const string ANIM_PARAM_NAME_FLEX = "Flex";
        public const string ANIM_PARAM_NAME_POSE = "Pose";
        public const float THRESH_COLLISION_FLEX = 0.9f;

        public const float INPUT_RATE_CHANGE = 20.0f;

        public const float COLLIDER_SCALE_MIN = 0.01f;
        public const float COLLIDER_SCALE_MAX = 1.0f;
        public const float COLLIDER_SCALE_PER_SECOND = 1.0f;

        public const float TRIGGER_DEBOUNCE_TIME = 0.05f;
        public const float THUMB_DEBOUNCE_TIME = 0.15f;

        [SerializeField]
        private OVRInput.Controller m_controller = OVRInput.Controller.None;
        [SerializeField]
        private Animator m_animator = null;
        [SerializeField]
        private HandPose m_defaultGrabPose = null;

        private Collider[] m_colliders = null;
        private bool m_collisionEnabled = true;
        private OVRGrabber m_grabber;
        public VRTK_InteractGrab vrtk_grabber;
        [Header("Interaction Volumes")]
        public GameObject precisionVolume;
        public GameObject generalVolume;

        List<Renderer> m_showAfterInputFocusAcquired;

        private int m_animLayerIndexThumb = -1;
        private int m_animLayerIndexPoint = -1;
        private int m_animParamIndexFlex = -1;
        private int m_animParamIndexPose = -1;

        private bool m_isPointing = false;
        private bool m_isGivingThumbsUp = false;
        private float m_pointBlend = 0.0f;
        private float m_thumbsUpBlend = 0.0f;

        private bool m_restoreOnInputAcquired = false;

        private VRTK_ControllerEvents vrtkControllerEvents;
        private void Awake()
        {
            m_grabber = GetComponent<OVRGrabber>();
            vrtk_grabber = GetComponentInParent<VRTK_InteractGrab>();
            vrtkControllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
            precisionVolume.SetActive(false);
            generalVolume.SetActive(true);
        }
        private bool _indexPressed = false;
        private bool indexPressed
        {
            get
            {
                return _indexPressed;
            }
            set
            {
                _indexPressed = value;
                HandlePrecisionGrab();
            }
        }

        private bool _thumbPressed = false;
        private bool thumbPressed
        {
            get
            {
                return _thumbPressed;
            }
            set
            {
                _thumbPressed = value;
                HandlePrecisionGrab();
            }
        }

        private bool disablePrecisionGrab = false;

        private void SetPrecisionGrabbing(bool value)
        {
            disablePrecisionGrab = !value;
            precisionVolume.SetActive(value);
            generalVolume.SetActive(!value);
        }

        private IEnumerator TryToGrab()
        {
            var grabbing = vrtk_grabber.GetGrabbedObject();
            while (grabbing == null)
            {
                Debug.Log("<color=green>Attempting precision grab</color>");
                vrtk_grabber.AttemptGrab();
                grabbing = vrtk_grabber.GetGrabbedObject();
                yield return 0;
            }
        }
        private Coroutine grabbingCoroutine;
        private void HandlePrecisionGrab()
        {
            if (disablePrecisionGrab)
            {
                return;
            }
            if (thumbPressed && indexPressed)
            {
                precisionVolume.SetActive(true);
                generalVolume.SetActive(false);
                StartCoroutine("TryToGrab");
            }
            else
            {
                precisionVolume.SetActive(false);
                generalVolume.SetActive(true);
                vrtk_grabber.ForceRelease();
                StopCoroutine("TryToGrab");
            }
        }

        private void Start()
        {
            m_showAfterInputFocusAcquired = new List<Renderer>();

            // Collision starts disabled. We'll enable it for certain cases such as making a fist.
            m_colliders = this.GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
            CollisionEnable(false);
            vrtkControllerEvents.TouchpadTouchStart += (object sender, ControllerInteractionEventArgs e) => { thumbPressed = true; };
            vrtkControllerEvents.TouchpadTouchEnd += (object sender, ControllerInteractionEventArgs e) => { thumbPressed = false; };

            vrtkControllerEvents.TriggerPressed += (object sender, ControllerInteractionEventArgs e) => { indexPressed = true; };
            vrtkControllerEvents.TriggerReleased += (object sender, ControllerInteractionEventArgs e) => { indexPressed = false; };

            vrtk_grabber.GrabButtonPressed += (object sender, ControllerInteractionEventArgs e) => { disablePrecisionGrab = true; };
            vrtk_grabber.GrabButtonReleased += (object sender, ControllerInteractionEventArgs e) => { disablePrecisionGrab = false; };
            // Get animator layer indices by name, for later use switching between hand visuals
            m_animLayerIndexPoint = m_animator.GetLayerIndex(ANIM_LAYER_NAME_POINT);
            m_animLayerIndexThumb = m_animator.GetLayerIndex(ANIM_LAYER_NAME_THUMB);
            m_animParamIndexFlex = Animator.StringToHash(ANIM_PARAM_NAME_FLEX);
            m_animParamIndexPose = Animator.StringToHash(ANIM_PARAM_NAME_POSE);

            OVRManager.InputFocusAcquired += OnInputFocusAcquired;
            OVRManager.InputFocusLost += OnInputFocusLost;
#if UNITY_EDITOR
            OVRPlugin.SendEvent("custom_hand", (SceneManager.GetActiveScene().name == "CustomHands").ToString(), "sample_framework");
#endif
        }

        private void OnDestroy()
        {
            OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
            OVRManager.InputFocusLost -= OnInputFocusLost;
        }

        private void Update()
        {
            UpdateCapTouchStates();

            m_pointBlend = InputValueRateChange(m_isPointing, m_pointBlend);
            m_thumbsUpBlend = InputValueRateChange(m_isGivingThumbsUp, m_thumbsUpBlend);

            float flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

            bool collisionEnabled = vrtk_grabber.GetGrabbedObject() == null && flex >= THRESH_COLLISION_FLEX;
            // CollisionEnable(collisionEnabled);


            UpdateAnimStates();
        }

        // Just checking the state of the index and thumb cap touch sensors, but with a little bit of
        // debouncing.
        private void UpdateCapTouchStates()
        {
            m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, m_controller);
            m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, m_controller);
        }

        private void LateUpdate()
        {
            // Hand's collision grows over a short amount of time on enable, rather than snapping to on, to help somewhat with interpenetration issues.
            if (m_collisionEnabled && m_collisionScaleCurrent + Mathf.Epsilon < COLLIDER_SCALE_MAX)
            {
                m_collisionScaleCurrent = Mathf.Min(COLLIDER_SCALE_MAX, m_collisionScaleCurrent + Time.deltaTime * COLLIDER_SCALE_PER_SECOND);
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.transform.localScale = new Vector3(m_collisionScaleCurrent, m_collisionScaleCurrent, m_collisionScaleCurrent);
                }
            }
        }

        // Simple Dash support. Just hide the hands.
        private void OnInputFocusLost()
        {
            if (gameObject.activeInHierarchy)
            {
                m_showAfterInputFocusAcquired.Clear();
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; ++i)
                {
                    if (renderers[i].enabled)
                    {
                        renderers[i].enabled = false;
                        m_showAfterInputFocusAcquired.Add(renderers[i]);
                    }
                }

                CollisionEnable(false);

                m_restoreOnInputAcquired = true;
            }
        }

        private void OnInputFocusAcquired()
        {
            if (m_restoreOnInputAcquired)
            {
                for (int i = 0; i < m_showAfterInputFocusAcquired.Count; ++i)
                {
                    if (m_showAfterInputFocusAcquired[i])
                    {
                        m_showAfterInputFocusAcquired[i].enabled = true;
                    }
                }
                m_showAfterInputFocusAcquired.Clear();

                // Update function will update this flag appropriately. Do not set it to a potentially incorrect value here.
                //CollisionEnable(true);

                m_restoreOnInputAcquired = false;
            }
        }

        private float InputValueRateChange(bool isDown, float value)
        {
            float rateDelta = Time.deltaTime * INPUT_RATE_CHANGE;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }

        private void UpdateAnimStates()
        {
            // bool grabbing = m_grabber.grabbedObject != null;
            bool grabbing = vrtk_grabber.GetGrabbedObject() != null;
            HandPose grabPose = m_defaultGrabPose;
            if (grabbing)
            {
                // HandPose customPose = m_grabber.grabbedObject.GetComponent<HandPose>();
                HandPose customPose = vrtk_grabber.GetGrabbedObject().GetComponent<HandPose>();
                if (customPose != null) grabPose = customPose;
            }
            // Pose
            HandPoseId handPoseId = grabPose.PoseId;
            m_animator.SetInteger(m_animParamIndexPose, (int)handPoseId);

            // Flex
            // blend between open hand and fully closed fist
            float flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
            m_animator.SetFloat(m_animParamIndexFlex, flex);

            // Point
            bool canPoint = !grabbing || grabPose.AllowPointing;
            float point = canPoint ? m_pointBlend : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexPoint, point);

            // Thumbs up
            bool canThumbsUp = !grabbing || grabPose.AllowThumbsUp;
            float thumbsUp = canThumbsUp ? m_thumbsUpBlend : 0.0f;
            m_animator.SetLayerWeight(m_animLayerIndexThumb, thumbsUp);

            float pinch = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
            m_animator.SetFloat("Pinch", pinch);
        }

        private float m_collisionScaleCurrent = 0.0f;

        private void CollisionEnable(bool enabled)
        {
            if (m_collisionEnabled == enabled)
            {
                return;
            }
            m_collisionEnabled = enabled;

            if (enabled)
            {
                m_collisionScaleCurrent = COLLIDER_SCALE_MIN;
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
                    collider.enabled = true;
                }
            }
            else
            {
                m_collisionScaleCurrent = COLLIDER_SCALE_MAX;
                for (int i = 0; i < m_colliders.Length; ++i)
                {
                    Collider collider = m_colliders[i];
                    collider.enabled = false;
                    collider.transform.localScale = new Vector3(COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN, COLLIDER_SCALE_MIN);
                }
            }
        }
    }
}
