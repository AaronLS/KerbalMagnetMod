using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace CertifiedSilly.MagneticRings
{

    public interface IChamberCoupler {
        void Decouple();
    }

    public class ChamberDockingModule : ModuleDockingNode, IChamberCoupler
    {
        void IChamberCoupler.Decouple()
        {
            this.Decouple();
            this.Undock();
        }
    }

   
}
