using UnityEngine;
using System.Linq;

namespace CertifiedSilly.MagneticRings
{
    // DockedVesselInfo
    // ModuleDockingNode

    // TODO Aaron: Implement Variant sizes such as in Mobile Frame System mod where a right click editor cycles through sizes of part
    // TODO Aaron: Look at Kerbal Attachment Systems, Stork Delivery, or Infernal Robotics for ways to better clamp/mount


    public interface IMagnet
    {
        void ActivateMagnet();
        void DeactivateMagnet();
        bool ShouldAutoShutdown { get; set; }
        bool IsMagnetOn { get; set; }
    }

    public class MagneticRingModule: PartModule, IMagnet  // ModuleDockingNode
    {


        #region MagnetStatus
        
        [KSPField(isPersistant = true)]
        public bool isMagnetOn;
        public bool IsMagnetOn { get { return isMagnetOn; } set { isMagnetOn = value; } }
        
        [KSPField(isPersistant = false, guiActive = true, guiName = "Magnet Status", category="zMagnetStatus")]
        public string MagnetStatusName;

        // Part right-click menu
        [KSPEvent(guiActive = true, guiName = "Toggle Magnet", name = "ToggleMagnet", category="aaaToggleMagnet")] // CONSIDER: Enabling external access, but maybe better if we made a remote control module that you can put on remote vessels.  , externalToEVAOnly= false, guiActiveUnfocused = true, unfocusedRange=50)]
        public void ToggleMagnet()
        {
            isMagnetOn = !isMagnetOn;
            MagnetStatusName = (isMagnetOn ? "On" : "Off");

            if (isMagnetOn)
                this.part.force_activate();
            else
                this.part.deactivate();

            //ScreenMessages.PostScreenMessage("Magnet: " + (isMagnetOn ? "On" : "Off"), 5.0f, ScreenMessageStyle.UPPER_LEFT);
        }

        // KSPAction shows up in action groups
        [KSPAction("Toggle Magnet")]
        public void ToggleMagnetAction(KSPActionParam param)
        {
            if (param.type == KSPActionType.Activate)
                ActivateMagnet();
            else if (param.type == KSPActionType.Deactivate)
                DeactivateMagnet();
        }

       // [KSPEvent(guiActive = true, guiName = "Activate Magnet", name= "ActivateMagnet",guiActiveUnfocused = true)]
        public void ActivateMagnet()
        {
            if (!isMagnetOn)
                ToggleMagnet();
        }

       // [KSPEvent(guiActive = true, guiName = "Deactivate Magnet", name = "DeactivateMagnet", guiActiveUnfocused = true,active = false)]
        public void DeactivateMagnet()
        {
            if (isMagnetOn)
                ToggleMagnet();
        }

        [KSPField(isPersistant = true, guiActive = true, guiName = "AutoOff")
        , UI_Toggle(disabledText="Disabled", enabledText="Enabled")]
        public bool shouldAutoShutdown = true;

        public bool ShouldAutoShutdown
        {
            get { return shouldAutoShutdown; }
            set { shouldAutoShutdown = value; }
        }
        


        //private void setEventGui(string eventSuffix, bool isActivating)
        //{
        //    Events["Activate" + eventSuffix].active = !isActivating;
        //    Events["Deactivate" + eventSuffix].active = isActivating;
        //}

        #endregion

        #region Magnet Pull Force & Electricity Usage
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Kilowatts Draw") 
        , UI_FloatRange(minValue = 0f, maxValue = 10000f, stepIncrement = 100f)]
        public float kilowatts= 10000;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Force Multiplier")
            , UI_FloatRange(minValue = 0f, maxValue = 100f, stepIncrement = 1f)]
        public float PullForceMultiplier = 1;

        // Convert kilowatts to kilonetuns
        [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = false, guiName = "Kilonewtons Pull Force at Center") ]
        public float PullForceKilonewtons { get { return kilowatts * 0.0628673134f * PullForceMultiplier; } }

        

        // See for magnet strengths: https://www.kjmagnetics.com/magnetsummary.asp
        // Estimate pull force of 45T magnet(strongest in world) to be 20000 lbs. 30 MW(megawatts) 30,000,000 watts
        // Weighs 35 tons
        // https://nationalmaglab.org/about/around-the-lab/meet-the-magnets/meet-the-45-tesla-hybrid-magnet

        // The outer ring of the magnet is 11.5 T, 125231 lbs, 7,500,000 watts
        // The outer ring of the magnet is 10 T, 95000 lbs(421.211001 kilonewtons), 6,700,000 watts.  0.0628673134 kilo newtons per kilowatts    
        // Weighs ~7 tons

        // Amount of watts used in one second is dvidied by 3600.  (10,000,000 watts is 2700 watts/sec)

        // Battery capacity unit == 1 watt hour

        //- KSP units
        //OX4 solar panel (production) ElectricCharge 2/sec
        //RTG (production) ElectricCharge 60/min
        //ion engine (consumption) ElectricCharge 12/sec
        //illuminator MK1 spotlight (consumption) ElectricCharge 2.4/min
        //Z500 battery (storage) ElectricCharge 500

        //Normalize all time units to hours: if VAB info shows per-second multiply by 3600, if info shows per-minute multiply by 60.
        //Remove the time unit from parts that consume or produce electric power, apply time unit (hours) to batteries (storage). 
        //Use "kilo" where applicable:

        //OX4 solar panel (production) 7.2 kiloWatt
        //RTG (production) 3.6 kiloWatt
        //ion engine (consumption) 43.2 kiloWatt (*
        //Illuminator MK1 spotlight (consumption) 144 Watt
        //Z500 battery (storage) 500Watt-hour (0.5 kiloWatt-hour) 

        //On the face of it these numbers are fairly realistic, until we look at how solar panel energy production compares to panel size.

        //7 kiloWatt is rather a lot for a panel measuring ~2m by ~0.4m (less than 1 meter square), even for 'NASA quality' panels. The sun delivers only a little over 1kW per square meter on Earth. http://earthobservatory.nasa.gov/Features/SORCE/


           // Force lb.s = T^2 * 946.928


        // PULSE MAGNETS
        // 100T pulse magnet for 15 milliseconds, .02 seconsd
        #endregion


        #region old fields
        //[KSPField(isPersistant = false, guiActive = false)]
        //public float resourceAmount;//{ get; set; }

        //[KSPField(isPersistant = false, guiActive = false)]
        //public string resourceName;//{get; set;}

        //[KSPField(isPersistant = true, guiActive = false)]
        //public bool generatorActive;

        ////consume this resource per game-second
        //[KSPField(isPersistant = false, guiActive = false)]
        //public float generatorResourceIn;
        ////produce this resource per game second
        //[KSPField(isPersistant = false, guiActive = false)]
        //public float generatorResourceOut;

        //[KSPField(isPersistant = false, guiActive = false)]
        //public string generatorResourceInName;
        //[KSPField(isPersistant = false, guiActive = false)]
        //public string generatorResourceOutName;
    

        //[KSPEvent(guiName = "Decouple Node", active = true, guiActiveUnfocused = true, externalToEVAOnly = true, guiActive = true, unfocusedRange = 2f)]
        //public void Decouple() { base.Decouple(); }
        //[KSPAction("Decouple Node")]
        //public void DecoupleAction(KSPActionParam param) { base.DecoupleAction(param); }



    //    nodeTransformName =           
    //controlTransformName = 
    //undockEjectionForce = 10f;          //how hard the base pushes the undocking vessel when undocking. Haven't tested.
    //minDistanceToReEngage = 1f;     //how far you have to back away before attempting to redock
    //acquireRange = 0.5f;                 //how close before magnetic force kicks in between the ports
    //acquireMinFwdDot = 0.7f;          //the max forward angle between the two port's Z axis for acquireForce to kick in. cosine of the angle, 0.7 is roughly 45 degrees
    //acquireMinRollDot = -3.40282347E+38f;   //not sure, haven't tested. don't think it matters since you can dock in any roll angle
    //acquireForce = 2f;          //how strong the magnetic pull is
    //acquireTorque = 2f;       //how strong the force is attempting to align the docking port's Z to the target port's Z axis.
    //captureRange = 0.06f;     //how close the dockingNodes has to be before game considers them "Docked"
    //captureMinFwdDot = 0.998f;         //the max angle between the two ports when docked, cosine of the angle. 1 = cosine 0 degrees
    //captureMinRollDot = -3.40282347E+38f;    
    //captureMaxRvel = 0.3f;      //max rel. velocity to dock and not bounce off.
    //referenceAttachNode = string.Empty;
    //nodeType =       //two ports of the same type can dock together, can be any string, not restricted to size0, 1, 2, etc.
    //deployAnimationController = -1;    // not sure what deployAnimationController parameters do.
        //deployAnimationTarget = 1f;
        #endregion

        #region debug field
        //[KSPField(isPersistant = true, guiActive = true, guiName = "UseLocalCenter")]
        //public bool UseLocalCenter;

        // Part right-click menu
        //[KSPEvent(guiActive = true, guiName = "Toggle UseLocalCenter", name = "ToggleUseLocalCenter", guiActiveUnfocused = true)]
        //public void ToggleUseLocalCenter()
        //{
        //    UseLocalCenter = !UseLocalCenter;            
        //}

        [KSPField(isPersistant = false, guiActive = true, guiName = "Relative Velocity")]
        public string RelativeVelocityField;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Projected on (B-A)")]
        //public string ComponentRelativeVelocityFieldBminusA;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Perpendicular(B-A)")]
        //public string PerpendicularRelativeVelocityFieldBminusA;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Velocity")]
        //public string VelocityField;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Other Velocity")]
        //public string OtherVelocityField;



        //[KSPField(isPersistant = false, guiActive = true, guiName = "DotProductComponent")]
        //public string DotProductAComp;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Distance Perpendicular")]
        //public string DistancePerpendiculaBMinusA;

        //[KSPField(isPersistant = false, guiActive = true, guiName = "Shutdown")]
        //public string Shutdown;


        //[KSPField(isPersistant = false, guiActive = true, guiName = "Perpendicular(A-B)")]
        //public string PerpendicularRelativeVelocityFieldAminusB;

        #endregion

        private bool eventsAdded = false;
        public override void OnStart(PartModule.StartState state)
        {
            

            print("StartState: " + state.ToString());
            base.OnStart(state);
                  
            centerTransform = this.part.FindModelTransform("model");

            if(state != StartState.Editor)
            {
                MagnetStatusName = (isMagnetOn ? "On" : "Off");

                if (isMagnetOn)
                    this.part.force_activate();
                else
                    this.part.deactivate();
            }

            if (!eventsAdded)
            {
                GameEvents.onVesselRecoveryProcessing.Add(OnVesselRecoveryProcessing);
                
            }

            #region fact finding
            //list transforms
            //var transforms = this.GetComponents<Transform>();

            // WARNING: this reutrns NULL
            //centerTransform = this.part.FindModelTransform("MagneticDockingRing");

            //centerTransform = this.part.FindModelTransform("launchAccelerator1");
                          

            //if (centerTransform != null)
            //{
            //    // only returns the "model" transform itself
            //    //centerTransform.GetComponents<Transform>()
             
            //    print("model Transform .GetComponentsInChildren<Transform>()...");
            //    foreach (var transform in centerTransform.GetComponentsInChildren<Transform>())
            //    {
            //        //model
            //        //Aaron/Parts/MagneticDockingRing/model(Clone)
            //        //launchAccelerator1
            //        //Cube
            //        //Cylinder
            //        //Cylinder_001
            //        //Torus
            //        // Torus_001 - Torus_010
            //        //LoadingPoint

            //        printTransform(transform);
            //        print(transform.position == null ? "null" : "non-null");
            //        print(transform.root == null ? "null root" : "non-null root");
            //    }


            //}
            #endregion

        }

        private Transform centerTransform;       
        //private const int updateResolution = 1;
        int updateNth = 0;

        void OnVesselRecoveryProcessing(ProtoVessel vessel, MissionRecoveryDialog recoveryDialog, float blah)
        {
            print("Vessel Name(ISRecoverable:float): " + vessel.vesselName + "(" + vessel.vesselRef.IsRecoverable + ":" + blah + ")");
            print("Recovery Dialog: " + string.Join(" |\r\n ", 
                recoveryDialog.GetType().GetMembers(System.Reflection.BindingFlags.NonPublic).Select(m=>  m.MemberType+": "+ m.ReflectedType + " "+ m.Name).ToArray()));


            // https://code.google.com/p/ssaad/source/browse/

        //  public void handleRecoveryProcessing(ProtoVessel pv, MissionRecoveryDialog d, float f) {
        //  foreach (JMPScienceSubject s in scienceData.Keys) {
        //      float sci = ResearchAndDevelopment.GetScienceValue(scienceData[s], s.subject);
        //      ResearchAndDevelopment.Instance.SubmitScienceData(scienceData[s], s.subject);
        //
        //      d.AddDataWidget(new MissionRecoveryDialog.ScienceSubjectWidget(s.subject, scienceData[s], sci));
        //  }
        //
        //  scienceData.Clear();
        //  capacityUsed = 0;
        //  part.RequestResource("DataStorage", -capacity);
        //
        //  GameEvents.onVesselRecoveryProcessing.Remove(handleRecoveryProcessing);
        //  }

            // Figured it out, I have to call ResearchAndDevelopment.GetSubjectByID() during recovery processing to get the ScienceSubject to do stuff to.

            //  ProtoVessel => ProtoPartSnapshot => ProtoPartModuleSnapshot => ConfigNode = ProtoPartModuleSnapshot.moduleValues => ConfigNode.GetValue("nameOfYourField")


        //     public void onVesselRecovered(ProtoVessel vessel)        
        //    {
        //    List<ProtoPartSnapshot> partList = vessel.protoPartSnapshots;
        //    foreach (ProtoPartSnapshot a in partList)
        //    {
        //        List<ProtoPartModuleSnapshot> modules = a.modules;
        //        foreach (ProtoPartModuleSnapshot module in modules)
        //        {


        //            if (module.moduleName == "DeepFreezer")
        //            {
        //                ConfigNode node = module.moduleValues;
        //                string FrozenCrew = node.GetValue("FrozenCrew");
        //                ThawFrozenCrew(FrozenCrew);
        //            }
        //        }
        //    }
        //}
        //public void ThawFrozenCrew(String FrozenCrew)
        //{
        //    List<String> StoredCrew = FrozenCrew.Split(',').ToList();
        //    foreach (string frozenkerbal in StoredCrew)
        //    {
        //        foreach (ProtoCrewMember kerbal in HighLogic.CurrentGame.CrewRoster.Crew) //There's probably a more efficient way to find Protocrewmember from the CrewRoster
        //        {
        //            if (kerbal.name == frozenkerbal)
        //            {
        //                kerbal.rosterStatus = ProtoCrewMember.RosterStatus.Available;
        //                ScreenMessages.PostScreenMessage(kerbal.name + " was found in and thawed out.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
        //            }
        //        }
        //    }
        //}


//            void RecoveryProcessingCallback(ProtoVessel pv, MissionRecoveryDialog dlg, float recovery)
//{
//   float repEarned = dlg.reputationEarned;
//   float sciEarned = dlg.scienceEarned;
//   double fundsEarned = dlg.fundsEarned;
//   Debug.Log("[Recovery Timer] Rep: " + repEarned);
//   Debug.Log("[Recovery Timer] Sci: " + sciEarned);
//   Debug.Log("[Recovery Timer] Funds: " + fundsEarned);

//   double currentUniversalTime = Planetarium.GetUniversalTime();
//   Debug.Log("[Recovery Timer] Time of recovery: " + currentUniversalTime);
//}

        }


        // we use `void FixedUpdate()` instead of the override so that our part will run without being activated
        //public void FixedUpdate()
        public override void OnFixedUpdate()
        {
            ++updateNth;
            if (updateNth == int.MaxValue)
                updateNth = 0;

            if(isMagnetOn)
            {
                foreach (Vessel otherVessel in FlightGlobals.Vessels.Where(v => v.loaded))
                {
                    if (otherVessel.id == this.vessel.id)
                        continue;

                    Vector3d vesselPosition = otherVessel.findWorldCenterOfMass();

                    float distance = Vector3.Distance(centerTransform.position, vesselPosition);
                    if (Vector3.Distance(centerTransform.position, vesselPosition) < 1000f)
                    {
                        if (!shouldShutdown(otherVessel, centerTransform.position))
                        {
                            // Force decreases exponentially with distance.  D ^ 1.3 is a rough simplification that is a bit more forgiving of distance
                            Vector3d forceVector =
                                (centerTransform.position - vesselPosition).normalized * PullForceKilonewtons
                                / (Mathf.Pow(distance, 1.3f) + 1f);

                            // We need to make sure we apply the force to the center of mass (AddForce finds the center of the vessel, which is not the same as center of mass and thus will cause rotation)
                            otherVessel.rigidbody.AddForceAtPosition(forceVector, vesselPosition);
                            // and apply equal&opposite to ourself
                            this.vessel.rigidbody.AddForceAtPosition(-forceVector, this.vessel.findWorldCenterOfMass());
                        }
                    }
                }
            }

            base.OnFixedUpdate();
        }

        private bool shouldShutdown(Vessel attractedVessel, Vector3 magnetCenter)
        {
            if (shouldAutoShutdown)
            {
                // this.vessel and magnetCenter == vessel A
                // attractedVessel == vessel B
                // (A-B) gives vector pointing from B to A

                //  TimeWarp.fixedDeltaTime


                // The vessel could be on a trajectory that will pass outside us.
                // We want to know if it will pass through us, so we will get the relative velocity            
                // then decompose it into the component vector pointing towards us            
                var relativeVelocityBminusA = attractedVessel.rb_velocity - vessel.rb_velocity;
                var relativeDistanceBminusA = attractedVessel.findWorldCenterOfMass() - magnetCenter;

                // We'll be able to compare the relative Velocity with the distance to tell if B is moving towards or away from us
                // When B is moving towards the magnet, the vector pointing from B to A will be close to the same direction as the relative velocity pointing from B to A
                // When B is moving away from the magnet, the vector point from B to A will be roughly opposite of the relative velocity which will be pointing from A to B since B is moving away from A.
                // We can use the DOT product between relative velocity and and relative distance to determine if it's moving torward or away. (negative or positive:away)

                // Decompose vectors into just the components in the direction of the part's .up vector(i.e. the vector pointing through the ring)
                //Vector3 velocityComponentTowardsMagnetBminusA = Vector3.Project(relativeVelocityBminusA, (attractedVessel.findWorldCenterOfMass() - magnetCenter).normalized);
                Vector3 velocityComponentTowardsMagnetBminusA = Vector3.Project(relativeVelocityBminusA, centerTransform.up);
                Vector3 distanceComponentTowardsMagnetBminusA = Vector3.Project(relativeDistanceBminusA, centerTransform.up);

                var velocityPerpendiculaBMinusA = relativeVelocityBminusA - velocityComponentTowardsMagnetBminusA;



                if (Vector3.Dot(velocityComponentTowardsMagnetBminusA, distanceComponentTowardsMagnetBminusA) > 0)
                {
                    // Is moving away on the axis of the ring's center.  Assuming this is the first frame of moving away, then it just passed through the plane of the magnet.
                    // Either it passed through the center of the ring or outside the ring.

                    // Verify we passed through plane in last time step: Did we travel more than the distance in the last time step? (we'll look back two timesteps just to allow for error
                    if ((velocityComponentTowardsMagnetBminusA * TimeWarp.fixedDeltaTime).magnitude * 2 > distanceComponentTowardsMagnetBminusA.magnitude)
                    {
                        // See how far off center we are.

                        var distancePerpendiculaBMinusA = relativeDistanceBminusA - distanceComponentTowardsMagnetBminusA;

                        // Are we within 5 meters of the center?  This ensures objects happening by don't shutdown the magnet
                        if (distancePerpendiculaBMinusA.magnitude < 6)
                        {
                            DeactivateMagnet();
                            return true;
                        }
                    }
                }

                if (updateNth % printFrequency == 0)
                {
                    RelativeVelocityField = relativeVelocityBminusA.magnitude.ToString("0.00");
                }

            }
            return false;
            

            

        }


        const int printFrequency = 50;
        private void printIntermittant(string message)
        {
            if (updateNth % printFrequency == 0)
                print(message);
        }

        private void printIntermittantFormatted(string format, params object[] args)
        {
            if (updateNth % printFrequency == 0)
                print(string.Format(format, args));
        }

        private static void printFormatted(string format, params object[] args)
        {            
                print(string.Format(format, args));
        }

        public static void printTransform(Transform transform)
        {
            print("Transform: " + transform.name);
        }

        
        // Dynamic modify UI
        //var range = (UI_FloatRange)this.Fields["tweakScale"].uiControlEditor;
        //range.minValue = configValue("minScale", defaultValue: isFreeScale ? 0.5f : (float)scaleFactors.First());
        //range.maxValue = configValue("maxScale", defaultValue: isFreeScale ? 2.0f : (float)scaleFactors.Last());
        //range.stepIncrement = configValue("stepIncrement", defaultValue: 0.01f);
        
        
        
        //public static int Find(Transform part, string name, ref List<Transform> aTransform)
        //{


        //    IEnumerator enumerator = part.GetEnumerator();
        //    try
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            Transform part1 = (Transform)enumerator.Current;
        //            if (part1.name.StartsWith(name))
        //                aTransform.Add(part1);
        //            sfrUtility.Find(part1, name, ref aTransform);
        //        }
        //    }
        //    finally
        //    {
        //        IDisposable disposable = enumerator as IDisposable;
        //        if (disposable != null)
        //            disposable.Dispose();
        //    }
        //    return part.childCount;
        //}

       // private void PlayStartAnimation(Animation StartAnimation, string startAnimationName, int speed, bool instant)
       // {
       //     if (startAnimationName != "")
       //     {
       //         if (speed < 0)
       //         {
       //             StartAnimation[startAnimationName].time = StartAnimation[startAnimationName].length;
       //         }
       //         if (instant)
       //             StartAnimation[startAnimationName].speed = 999999 * speed;
       //         StartAnimation[startAnimationName].speed = speed;
       //         StartAnimation.Play(startAnimationName);
       //     }
       // }
       // public void PlayAnimation(string name, bool rewind, bool instant, bool loop)
       // {
       //     // note: assumes one ModuleAnimateGeneric (or derived version) for this part
       //     // if this isn't the case, needs fixing. That's cool, I called in the part.cfg
          
          
       //     {
               
       //         var anim = part.FindModelAnimators();
               
       //         foreach (Animation a in anim)
       //         {
       //            // print("animation found " + a.name + " " + a.clip.name);
       //             if (a.clip.name == name)
       //             {
       //                // print("animation playingxx " + a.name + " " + a.clip.name);
       //                 var xanim = a;
       //                 if (loop)
       //                     xanim.wrapMode = WrapMode.Loop;
       //                 else
       //                     xanim.wrapMode = WrapMode.Once;
       //                 PlayStartAnimation(xanim, name, (rewind) ? (-1) : (1), instant);
       //             }
       //         }
              
       //     }


       // }
       
       //  [KSPEvent(guiName = "Activate Generator", active = true, guiActive = true)]
       // public void activateGenerator()
       // {
       //     generatorActive = true;
       //     PlayAnimation("Deploy", false,false, false);
             

       // }
       //  [KSPEvent(guiName = "Activate Generator", active = true, guiActive = true)]
       //  public void deActivateGenerator()
       //  {
       //      generatorActive = false;
       //      PlayAnimation("Deploy", true, true, false);
       //  }
        
       // public override void OnStart(PartModule.StartState state)
       // {

       //     //this.Events["Deploy"].guiActive = false;
       //     Events["activateGenerator"].guiName = generatorActivateName;
       //     Events["deActivateGenerator"].guiName = generatorDeactivateName;

       //     if (generatorActive)
       //         PlayAnimation("Deploy", false, true, false);
       //     else
       //         PlayAnimation("Deploy", true, true, false);
       //     if(loopingAnimation != "")
       //         PlayAnimation(loopingAnimation, false, false, true); //plays independently of other anims
       //     base.OnStart(state);
       // }

       // public override void OnUpdate()
       // {
       //     int lcrewCount = part.protoModuleCrew.Count;
       //    if(generatorActive)
       //    {
       //        Events["deActivateGenerator"].guiActive = true;
       //        Events["activateGenerator"].guiActive = false;
       //        //while the generator is active... update the resource based on how much game time passed
       //        double dt = TimeWarp.deltaTime;
       //       // print("part has crews!" + part.protoModuleCrew.Count.ToString());
       //        if ((part.protoModuleCrew.Count == part.CrewCapacity && needSubjects) || !needSubjects)
       //        {
       //           // double budget = getResourceBudget(generatorResourceInName);
       //           // print(budget.ToString());
       //           // if (budget > 0)
       //            {
       //                double spent = part.RequestResource(generatorResourceInName, generatorResourceIn * dt);
       //              //  print(spent.ToString());
       //                double generatescale = spent / (generatorResourceIn * dt);
       //                if (generatorResourceIn == 0)
       //                    generatescale = 1;
       //                double generated = part.RequestResource(generatorResourceOutName, -generatorResourceOut * dt * generatescale);
       //                //  print("generated " + generated.ToString());
       //                if(generated == 0) //if we didn't generate anything then we're full, refund the spent resource
       //                    part.RequestResource(generatorResourceInName, -spent);
                       
       //            }
                     
                   
                  
                  
       //        }
       //    }
       //    else
       //    {
       //        Events["deActivateGenerator"].guiActive = false;
       //        Events["activateGenerator"].guiActive = true;
       //    }
       //    string biome = BiomeCheck();
       //     if(biome != currentBiome || lcrewCount != crewCount)
       //     {
       //         print("biome change " + biome);
       //         currentBiome = biome;
       //         crewCount = lcrewCount;
       //         //reset collected data
       //         part.RequestResource(resourceName, resourceAmount);


       //     }
       //     if (loopingAnimation != "")
       //         PlayAnimation(loopingAnimation, false, false, true); //plays independently of other anims
       //     base.OnUpdate();
          
       // }
       // public string BiomeCheck()
       // {

       //    // bool flying = vessel.altitude < vessel.mainBody.maxAtmosphereAltitude;
       //     //bool orbiting = 
               
       //             //return "InspaceOver" + vessel.mainBody.name;

       //     string situation = vessel.RevealSituationString();
       //            if(situation.Contains("Landed") || situation.Contains("flight"))
       //         return FlightGlobals.currentMainBody.BiomeMap.GetAtt(vessel.latitude * Mathf.Deg2Rad, vessel.longitude * Mathf.Deg2Rad).name + situation;


       //            return situation;
       // }
       // float getResourceBudget(string name)
       // {
       //     //   
       //     if (this.vessel == FlightGlobals.ActiveVessel)
       //     {
       //        // print("found vessel event!");
       //         var resources = vessel.GetActiveResources();
       //         for (int i = 0; i < resources.Count; i++)
       //         {
       //            // print("vessel has resources!");
       //             print(resources[i].info.name);
       //            // print("im looking for " + resourceName);
       //             if (resources[i].info.name == resourceName)
       //             {
       //                // print("Found the resouce!!");
       //                return (float)resources[i].amount;
                        
       //             }
       //         }
       //     }
       //     return 0;
       // }
       // bool vesselHasEnoughResource(string name, float rc)
       // {
       //   //   
       //     if (this.vessel == FlightGlobals.ActiveVessel)
       //     {
       //         print("found vessel event!");
       //         var resources = vessel.GetActiveResources();
       //         for (int i = 0; i < resources.Count; i++)
       //         {
       //             print("vessel has resources!");
       //             print(resources[i].info.name);
       //             print("im looking for " + resourceName);
       //             if (resources[i].info.name == resourceName)
       //             {
       //                 print("Found the resouce!!");
       //                 if (resources[i].amount >= resourceAmount)
       //                 {
       //                     return true;
       //                 }
       //             }
       //         }
       //     }
       //     return false;
       // }
        
       // new public void DumpData(ScienceData data)
       // {
       //    // refundResource();
       //     base.DumpData(data);


       // }
       //[KSPEvent(guiName = "Deploy", active = true, guiActive = true)]
       //new public void DeployExperiment()
       // {
       //     print("Clicked event! check data: " + resourceName + " " + resourceAmount.ToString() + " " + experimentID + " " );
       //     if (vesselHasEnoughResource(resourceName, resourceAmount))
       //     {

       //         print("Has the amount!!");
       //         double res = part.RequestResource(resourceName, resourceAmount, ResourceFlowMode.ALL_VESSEL);
       //         print("got " + res.ToString() + "resources");


       //         base.DeployExperiment();

       //       //  ReviewDataItem(data);
               

       //     }
       //     else
       //     {
       //         ScreenMessage smg = new ScreenMessage("Not Enough Data Stored", 4.0f, ScreenMessageStyle.UPPER_LEFT);
       //         ScreenMessages.PostScreenMessage(smg);
       //         print("not enough data stored");
       //     }
       //         print("Deploying Experiment");
       //         print("resourcename, resource amount " + resourceName + " " + resourceAmount.ToString());

                  
       // }

       //[KSPAction("Deploy")]
       //public void DeployAction(KSPActionParam actParams)
       //{
       //    print("Clicked event! check data: " + resourceName + " " + resourceAmount.ToString() + " " + experimentID + " ");
       //    if (vesselHasEnoughResource(resourceName, resourceAmount))
       //    {

       //        print("Has the amount!!");
       //        double res = part.RequestResource(resourceName, resourceAmount, ResourceFlowMode.ALL_VESSEL);
       //        print("got " + res.ToString() + "resources");


       //        base.DeployAction(actParams);

       //        //  ReviewDataItem(data);


       //    }
       //    else
       //    {
       //        ScreenMessage smg = new ScreenMessage("Not Enough Data Stored", 4.0f, ScreenMessageStyle.UPPER_LEFT);
       //        ScreenMessages.PostScreenMessage(smg);
       //        print("not enough data stored");
       //    }
       //    print("Deploying Experiment");
       //    print("resourcename, resource amount " + resourceName + " " + resourceAmount.ToString());


       //}


       // //[KSPEvent(active = true, guiActive = true, guiName = "Review Data")]
       // //new public void ReviewDataEvent()
       // //{
       // //    print("Reviewing Data");
       // //    base.ReviewDataEvent();
       // //}
       // void refundResource()
       //{
       //    print("refund resource!");
       //    double res = part.RequestResource(resourceName, -resourceAmount, ResourceFlowMode.ALL_VESSEL);
       //    print("refunded " + res.ToString() + " resource");
       //}
       //[KSPEvent(guiName = "Reset", active = true, guiActive = true)]
       //new public void ResetExperiment()
       // {
       //    // refundResource();
       //     base.ResetExperiment();
       // }
       //[KSPEvent(guiName = "Reset", active = true, guiActiveUnfocused = true, externalToEVAOnly = true, guiActive = false)]
       //new public void ResetExperimentExternal()
       //{
       //  //  refundResource();
       //    base.ResetExperimentExternal();
       //}

       //[KSPAction("Reset")]
       //new public void ResetAction(KSPActionParam actParams)
       //{
       //    //refundResource();
       //    base.ResetAction(actParams);
       //}
        
    }
   
}
