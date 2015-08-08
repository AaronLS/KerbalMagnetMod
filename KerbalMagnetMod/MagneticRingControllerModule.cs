using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace CertifiedSilly.MagneticRings
{
    public class MagneticRingControllerModule:PartModule
    {


        #region MagnetStatus

        public override void OnAwake()
        {
            print("Controller: OnAwake()");
            base.OnAwake();
        }

        public override void OnActive()
        {
            print("Controller: OnActive()");
            base.OnActive();
        }

        //public enum OnOff { On, Off }
        //[KSPField(isPersistant = true, guiActive= true, guiName= "Safety")]
        //public OnOff Safety = OnOff.On;

        [KSPField(isPersistant = true, guiActive= true, guiName= "Safety")
        , UI_Toggle(disabledText = "Off", enabledText = "On")]
        public bool Safety = true;


        //[KSPField(isPersistant = true)]
        //public bool isMagnetOn;
        
        //[KSPField(isPersistant = false, guiActive = true, guiName = "Magnet Status")]
        //public string MagnetStatusName;
                
        // TODO: Enable this only for more complex controller.
        //[KSPEvent(guiActive = true, guiName = "Toggle All Magnets", name = "ToggleAllMagnets", guiActiveUnfocused = true)]
        //public void ToggleAllMagnets()
        //{
        //    if (Safety)
        //        ScreenMessages.PostScreenMessage("Controls are disabled. Turn off safety to operate. Debris will be attracted.");
        //    else {
        //        foreach (IMagnet magnet in GetAllMagnetParts(vessel))
        //        {
        //            if (magnet.IsMagnetOn)
        //                magnet.DeactivateMagnet();
        //            else
        //                magnet.ActivateMagnet();

        //        }
        //    }
        //}

        // TODO Aaron: Add KSPAction's for all gui items
        // KSPAction shows up in action groups                
        //[KSPAction("Toggle Magnet")]
        //public void MagnetAction(KSPActionParam param)
        //{
        //    if (param.type == KSPActionType.Activate)
        //        ActivateMagnet();
        //    else if (param.type == KSPActionType.Deactivate)
        //        DeactivateMagnet();
        //}


        //[KSPEvent(guiActive = true, guiName = "AutoOff: Enable All", name = "EnableAllAutoShutdown")]
        //public void EnableAllAutoShutdown()
        //{
        //    GetAllMagnetParts(vessel).ForEach(p => p.ShouldAutoShutdown = true);
        //}

        //[KSPEvent(guiActive = true, guiName = "AutoOff: Disable All", name = "DisableAllAutoShutdown")]
        //public void DisableAllAutoShutdown()
        //{
        //    GetAllMagnetParts(vessel).ForEach(p => p.ShouldAutoShutdown = false);
        //}


        [KSPEvent(guiActive = true, guiName = "All Magnets: Activate", name = "ActivateAllMagnets", guiActiveUnfocused = true)]
        public void ActivateAllMagnets()
        {
            if (Safety)
                ScreenMessages.PostScreenMessage("Controls are disabled. Turn off safety to operate. Debris will be attracted.");
            else {
                GetAllMagnetParts(vessel).ForEach(p => p.ActivateMagnet());
            }
        }

        [KSPEvent(guiActive = true, guiName = "All Magnets: Deactivate", name = "DeactivateAllMagnets")]
        public void DeactivateAllMagnets()
        {
             GetAllMagnetParts(vessel).ForEach(p => p.DeactivateMagnet());
        }

        [KSPEvent(guiActive = true, guiName = "Decouple & Fire All", name = "FireAllMagnets")]
        public void FireAllMagnets()
        {
            if (Safety)
                ScreenMessages.PostScreenMessage("Controls are disabled. Turn off safety to operate. Debris will be attracted.");
            else
            {
                GetAllMagnetParts(vessel).ForEach(p => p.ActivateMagnet());
                GetAllMagnetParts(vessel).ForEach(p => p.ShouldAutoShutdown = true);
                GetAllChamberParts(vessel).ForEach(p => p.Decouple());
            }
        }

        public static List<IMagnet> GetAllMagnetParts(Vessel vessel)
        {
            return vessel.FindPartModulesImplementing<IMagnet>().ToList();
        }

        public static List<IChamberCoupler> GetAllChamberParts(Vessel vessel)
        {
            return vessel.FindPartModulesImplementing<IChamberCoupler>().ToList();
        }

        //private void setEventGui(string eventSuffix, bool isActivating)
        //{
        //    Events["Activate" + eventSuffix].active = !isActivating;
        //    Events["Deactivate" + eventSuffix].active = isActivating;
        //}

        #endregion
                
        public override void OnStart(PartModule.StartState state)
        {
            print("StartState: " + state.ToString());
            base.OnStart(state);

            if(state != StartState.Editor)
            {
                //MagnetStatusName = (isMagnetOn ? "On" : "Off");
            }

        }
        


        private static void printFormatted(string format, params object[] args)
        {            
                print(string.Format(format, args));
        }

        public static void printTransform(Transform transform)
        {
            print("Transform: " + transform.name);
        }

        
    }
   
}
